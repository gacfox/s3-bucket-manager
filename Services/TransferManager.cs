using Amazon.S3;
using Amazon.S3.Model;
using Gacfox.S3BucketManager.Models;

namespace Gacfox.S3BucketManager.Services;

public class TransferManager
{
    private const int UploadPartSize = 8 * 1024 * 1024;
    private const int DownloadChunkSize = 4 * 1024 * 1024;

    private readonly ConnectionStore _store;
    private readonly SemaphoreSlim _uploadSlots;
    private readonly SemaphoreSlim _downloadSlots;
    private int _uploadConcurrency;
    private int _downloadConcurrency;

    public event Action<TransferTask>? TaskAdded;
    public event Action<TransferTask>? TaskUpdated;
    public event Action<TransferTask>? TaskFinished;
    public event Action? PersistRequested;

    private DateTime _lastPersistRequest;

    public TransferManager(ConnectionStore store)
    {
        _store = store;
        _uploadConcurrency = store.Settings.UploadConcurrency;
        _downloadConcurrency = store.Settings.DownloadConcurrency;
        _uploadSlots = new SemaphoreSlim(_uploadConcurrency);
        _downloadSlots = new SemaphoreSlim(_downloadConcurrency);
    }

    public void UpdateConcurrency(int upload, int download)
    {
        AdjustSlots(_uploadSlots, upload, ref _uploadConcurrency);
        AdjustSlots(_downloadSlots, download, ref _downloadConcurrency);
    }

    private static void AdjustSlots(SemaphoreSlim slots, int target, ref int current)
    {
        var diff = target - current;
        if (diff == 0) return;
        current = target;
        if (diff > 0)
        {
            slots.Release(diff);
        }
        else
        {
            // 后台永久占用多余槽位以降低并发，避免阻塞调用线程；
            // 这些 WaitAsync 永不 Release，等效于缩减信号量容量
            _ = Task.Run(async () =>
            {
                for (var i = 0; i < -diff; i++)
                    await slots.WaitAsync();
            });
        }
    }

    public void Enqueue(TransferTask task)
    {
        TaskAdded?.Invoke(task);
        RequestPersist(true);
        _ = RunAsync(task);
    }

    public void Restore(TransferTask task)
    {
        task.Status = TransferStatus.Paused;
        TaskAdded?.Invoke(task);
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
            RaiseUpdated(task, true);
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
        else if (task.Status == TransferStatus.Running)
        {
            // 立即刷新 UI 为"停止中…"，传输循环在分片边界感知 StopRequested 后结束
            RaiseUpdated(task, true);
        }
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
            else if (task.Status == TransferStatus.Paused)
                RaiseUpdated(task, true);
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
        var length = new FileInfo(task.LocalFilePath!).Length;
        if (task.UploadId != null && length != task.TotalBytes)
        {
            // 源文件在暂停期间被修改，丢弃已有分片状态重新上传
            task.UploadId = null;
            task.UploadedParts.Clear();
        }
        task.TotalBytes = length;
        if (task.UploadedParts.Count > 0)
            task.TransferredBytes = Math.Min(
                task.UploadedParts.Count * (long)UploadPartSize, task.TotalBytes);
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
            var partStart = task.TransferredBytes;
            var request = new UploadPartRequest
            {
                BucketName = task.BucketName,
                Key = task.Key,
                UploadId = task.UploadId,
                PartNumber = partNumber,
                PartSize = read,
                InputStream = new MemoryStream(buffer, 0, read)
            };
            request.StreamTransferProgress += (_, e) =>
            {
                var delta = partStart + e.TransferredBytes - task.TransferredBytes;
                if (delta <= 0) return;
                task.TransferredBytes += delta;
                RaiseUpdated(task);
            };
            var response = await client.UploadPartAsync(request);
            task.UploadedParts.Add(new PartETag(partNumber, response.ETag));
            task.TransferredBytes = partStart + read;
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
            if (task.CurrentFileOffset > 0)
            {
                var info = new FileInfo(localPath);
                if (!info.Exists || info.Length < task.CurrentFileOffset)
                {
                    // 部分文件缺失或被截短，该文件从头下载
                    task.TransferredBytes -= task.CurrentFileOffset;
                    task.CurrentFileOffset = 0;
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
            using (var stream = new FileStream(localPath,
                task.CurrentFileOffset == 0 ? FileMode.Create : FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.None))
            {
                stream.Seek(task.CurrentFileOffset, SeekOrigin.Begin);
                var copyBuffer = new byte[256 * 1024];
                while (task.CurrentFileOffset < size)
                {
                    if (CheckInterrupt(task)) return;
                    var start = task.CurrentFileOffset;
                    var end = Math.Min(start + DownloadChunkSize, size) - 1;
                    using var response = await client.GetObjectAsync(new GetObjectRequest
                    { BucketName = task.BucketName, Key = key, ByteRange = new ByteRange(start, end) });
                    int copied;
                    while ((copied = await response.ResponseStream.ReadAsync(copyBuffer, 0, copyBuffer.Length)) > 0)
                    {
                        await stream.WriteAsync(copyBuffer, 0, copied);
                        task.CurrentFileOffset += copied;
                        task.TransferredBytes += copied;
                        RaiseUpdated(task);
                        if (CheckInterrupt(task)) return;
                    }
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
        RequestPersist(force);
    }

    private void RequestPersist(bool force)
    {
        if (!force && (DateTime.UtcNow - _lastPersistRequest).TotalSeconds < 2) return;
        _lastPersistRequest = DateTime.UtcNow;
        PersistRequested?.Invoke();
    }

    private void FinishTask(TransferTask task)
    {
        task.FinishTime = DateTime.Now;
        TaskUpdated?.Invoke(task);
        TaskFinished?.Invoke(task);
    }
}
