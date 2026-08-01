using Amazon.S3;
using Amazon.S3.Model;
using Gacfox.S3BucketManager.Models;

namespace Gacfox.S3BucketManager.Services
{
    public class TransferManager
    {
        private const int UploadPartSize = 8 * 1024 * 1024;
        private const int DownloadChunkSize = 4 * 1024 * 1024;

        private readonly ConnectionStore _store;
        private SemaphoreSlim _uploadSlots;
        private SemaphoreSlim _downloadSlots;

        public event Action<TransferTask>? TaskAdded;
        public event Action<TransferTask>? TaskUpdated;
        public event Action<TransferTask>? TaskFinished;

        public TransferManager(ConnectionStore store)
        {
            _store = store;
            _uploadSlots = new SemaphoreSlim(store.Settings.UploadConcurrency);
            _downloadSlots = new SemaphoreSlim(store.Settings.DownloadConcurrency);
        }

        public void UpdateConcurrency(int upload, int download)
        {
            _uploadSlots = new SemaphoreSlim(upload);
            _downloadSlots = new SemaphoreSlim(download);
        }

        public void Enqueue(TransferTask task)
        {
            TaskAdded?.Invoke(task);
            _ = RunAsync(task);
        }

        public void Pause(TransferTask task)
        {
            if (task.Status == TransferStatus.Pending)
            {
                task.Status = TransferStatus.Paused;
                RaiseUpdated(task, true);
            }
            else if (task.Status == TransferStatus.Running)
            {
                task.PauseRequested = true;
            }
        }

        public void Resume(TransferTask task)
        {
            if (task.Status != TransferStatus.Paused) return;
            task.PauseRequested = false;
            task.Status = TransferStatus.Pending;
            RaiseUpdated(task, true);
            _ = RunAsync(task);
        }

        public void Stop(TransferTask task)
        {
            if (task.Status is TransferStatus.Completed or TransferStatus.Stopped or TransferStatus.Failed) return;
            task.StopRequested = true;
            if (task.Status == TransferStatus.Pending)
            {
                task.Status = TransferStatus.Stopped;
                FinishTask(task);
            }
            else if (task.Status == TransferStatus.Paused)
            {
                task.Status = TransferStatus.Stopped;
                FinishTask(task);
                DiscardPartialFile(task);
                _ = AbortMultipartAsync(task);
            }
            // Running 状态由传输循环在分片边界感知 StopRequested
        }

        private async Task RunAsync(TransferTask task)
        {
            var slots = task.Direction == TransferDirection.Upload ? _uploadSlots : _downloadSlots;
            await slots.WaitAsync();
            try
            {
                if (task.Status != TransferStatus.Pending) return;
                task.Status = TransferStatus.Running;
                RaiseUpdated(task, true);
                var credentials = _store.GetCredentials(task.Profile.Id);
                if (credentials == null)
                    throw new InvalidOperationException($"连接“{task.Profile.Name}”缺少凭据");
                using var client = S3ClientFactory.Create(task.Profile, credentials);
                if (task.Direction == TransferDirection.Upload)
                    await UploadCoreAsync(task, client);
                else
                    await DownloadCoreAsync(task, client);
                if (task.Status == TransferStatus.Stopped)
                {
                    await AbortMultipartAsync(task);
                    DiscardPartialFile(task);
                }
                if (task.Status == TransferStatus.Running)
                    task.Status = TransferStatus.Completed;
                if (task.Status is TransferStatus.Completed or TransferStatus.Stopped)
                    FinishTask(task);
            }
            catch (Exception ex)
            {
                task.Status = TransferStatus.Failed;
                task.ErrorMessage = ex.Message;
                FinishTask(task);
            }
            finally
            {
                slots.Release();
            }
        }

        private async Task UploadCoreAsync(TransferTask task, AmazonS3Client client)
        {
            task.TotalBytes = new FileInfo(task.LocalFilePath!).Length;
            if (task.TotalBytes == 0)
            {
                await client.PutObjectAsync(new PutObjectRequest
                { BucketName = task.BucketName, Key = task.Key, FilePath = task.LocalFilePath });
                return;
            }
            if (task.UploadId == null)
            {
                var init = await client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
                { BucketName = task.BucketName, Key = task.Key });
                task.UploadId = init.UploadId;
            }
            using var stream = File.OpenRead(task.LocalFilePath!);
            stream.Seek(task.TransferredBytes, SeekOrigin.Begin);
            var partNumber = task.UploadedParts.Count + 1;
            var buffer = new byte[UploadPartSize];
            while (task.TransferredBytes < task.TotalBytes)
            {
                if (CheckInterrupt(task)) return;
                var read = await stream.ReadAsync(buffer, 0,
                    (int)Math.Min(UploadPartSize, task.TotalBytes - task.TransferredBytes));
                if (read == 0) break;
                var response = await client.UploadPartAsync(new UploadPartRequest
                {
                    BucketName = task.BucketName,
                    Key = task.Key,
                    UploadId = task.UploadId,
                    PartNumber = partNumber,
                    PartSize = read,
                    InputStream = new MemoryStream(buffer, 0, read)
                });
                task.UploadedParts.Add(new PartETag(partNumber, response.ETag));
                task.TransferredBytes += read;
                partNumber++;
                RaiseUpdated(task);
            }
            if (CheckInterrupt(task)) return;
            await client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
            {
                BucketName = task.BucketName,
                Key = task.Key,
                UploadId = task.UploadId,
                PartETags = task.UploadedParts
            });
        }

        private async Task DownloadCoreAsync(TransferTask task, AmazonS3Client client)
        {
            if (task.DownloadFiles == null)
            {
                task.DownloadFiles = new List<KeyValuePair<string, long>>();
                if (task.SourcePrefix == null)
                {
                    task.DownloadFiles.Add(new KeyValuePair<string, long>(task.Key!, task.TotalBytes));
                }
                else
                {
                    string? token = null;
                    do
                    {
                        var response = await client.ListObjectsV2Async(new ListObjectsV2Request
                        {
                            BucketName = task.BucketName,
                            Prefix = task.SourcePrefix,
                            ContinuationToken = token
                        });
                        foreach (var obj in response.S3Objects ?? new List<S3Object>())
                        {
                            if (!obj.Key.EndsWith('/'))
                                task.DownloadFiles.Add(new KeyValuePair<string, long>(obj.Key, obj.Size ?? 0));
                        }
                        token = response.IsTruncated == true ? response.NextContinuationToken : null;
                    } while (token != null);
                    task.TotalBytes = task.DownloadFiles.Sum(f => f.Value);
                }
            }
            for (; task.DownloadFileIndex < task.DownloadFiles.Count; task.DownloadFileIndex++)
            {
                var (key, size) = task.DownloadFiles[task.DownloadFileIndex];
                var localPath = task.SourcePrefix == null
                    ? task.LocalTargetPath!
                    : Path.Combine(task.LocalTargetPath!,
                        key[task.SourcePrefix.Length..].Replace('/', Path.DirectorySeparatorChar));
                task.CurrentLocalPath = localPath;
                Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                using (var stream = new FileStream(localPath,
                    task.CurrentFileOffset == 0 ? FileMode.Create : FileMode.OpenOrCreate,
                    FileAccess.Write, FileShare.None))
                {
                    stream.Seek(task.CurrentFileOffset, SeekOrigin.Begin);
                    while (task.CurrentFileOffset < size)
                    {
                        if (CheckInterrupt(task)) return;
                        var start = task.CurrentFileOffset;
                        var end = Math.Min(start + DownloadChunkSize, size) - 1;
                        using var response = await client.GetObjectAsync(new GetObjectRequest
                        { BucketName = task.BucketName, Key = key, ByteRange = new ByteRange(start, end) });
                        await response.ResponseStream.CopyToAsync(stream);
                        task.CurrentFileOffset = end + 1;
                        task.TransferredBytes += end - start + 1;
                        RaiseUpdated(task);
                    }
                }
                task.CurrentFileOffset = 0;
                task.CurrentLocalPath = null;
            }
        }

        private async Task AbortMultipartAsync(TransferTask task)
        {
            if (task.Direction != TransferDirection.Upload || task.UploadId == null) return;
            try
            {
                var credentials = _store.GetCredentials(task.Profile.Id);
                if (credentials == null) return;
                using var client = S3ClientFactory.Create(task.Profile, credentials);
                await client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
                { BucketName = task.BucketName, Key = task.Key, UploadId = task.UploadId });
            }
            catch { }
            task.UploadId = null;
        }

        private static void DiscardPartialFile(TransferTask task)
        {
            if (task.Direction != TransferDirection.Download || task.CurrentLocalPath == null) return;
            try { File.Delete(task.CurrentLocalPath); } catch { }
            task.CurrentLocalPath = null;
        }

        private static bool CheckInterrupt(TransferTask task)
        {
            if (task.StopRequested)
            {
                task.Status = TransferStatus.Stopped;
                return true;
            }
            if (task.PauseRequested)
            {
                task.Status = TransferStatus.Paused;
                return true;
            }
            return false;
        }

        private void RaiseUpdated(TransferTask task, bool force = false)
        {
            if (!force && (DateTime.UtcNow - task.LastReportTime).TotalMilliseconds < 150) return;
            task.LastReportTime = DateTime.UtcNow;
            TaskUpdated?.Invoke(task);
        }

        private void FinishTask(TransferTask task)
        {
            task.FinishTime = DateTime.Now;
            TaskUpdated?.Invoke(task);
            TaskFinished?.Invoke(task);
        }
    }
}
