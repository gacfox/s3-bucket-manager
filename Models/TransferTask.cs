using Amazon.S3.Model;

namespace Gacfox.S3BucketManager.Models;

public enum TransferDirection
{
    Upload,
    Download
}

public enum TransferStatus
{
    Pending,
    Running,
    Paused,
    Completed,
    Stopped,
    Failed
}

public class TransferTask
{
    public Guid Id { get; } = Guid.NewGuid();
    public required TransferDirection Direction { get; init; }
    public required ConnectionProfile Profile { get; init; }
    public required string BucketName { get; init; }
    public required string DisplayName { get; init; }
    public long TotalBytes { get; internal set; }
    public long TransferredBytes { get; internal set; }
    public TransferStatus Status { get; internal set; } = TransferStatus.Pending;
    public string? ErrorMessage { get; internal set; }
    public DateTime FinishTime { get; internal set; }

    public string? LocalFilePath { get; init; }
    public string? Key { get; init; }
    public string? SourcePrefix { get; init; }
    public string? LocalTargetPath { get; init; }

    internal bool PauseRequested;
    internal bool StopRequested;
    internal string? UploadId;
    internal List<PartETag> UploadedParts { get; } = new();
    internal List<KeyValuePair<string, long>>? DownloadFiles;
    internal int DownloadFileIndex;
    internal long CurrentFileOffset;
    internal string? CurrentLocalPath;
    internal DateTime LastReportTime;

    public TransferTaskSnapshot ToSnapshot() => new()
    {
        Id = Id,
        Direction = (int)Direction,
        ConnectionId = Profile.Id,
        BucketName = BucketName,
        DisplayName = DisplayName,
        TotalBytes = TotalBytes,
        TransferredBytes = TransferredBytes,
        LocalFilePath = LocalFilePath,
        Key = Key,
        SourcePrefix = SourcePrefix,
        LocalTargetPath = LocalTargetPath,
        UploadId = UploadId,
        UploadedParts = UploadedParts
            .Select(p => new UploadPartSnapshot { PartNumber = p.PartNumber ?? 0, ETag = p.ETag }).ToList(),
        DownloadFiles = DownloadFiles?
            .Select(f => new DownloadFileSnapshot { Key = f.Key, Size = f.Value }).ToList(),
        DownloadFileIndex = DownloadFileIndex,
        CurrentFileOffset = CurrentFileOffset,
        CurrentLocalPath = CurrentLocalPath
    };

    public static TransferTask FromSnapshot(TransferTaskSnapshot snapshot, ConnectionProfile profile)
    {
        var task = new TransferTask
        {
            Direction = (TransferDirection)snapshot.Direction,
            Profile = profile,
            BucketName = snapshot.BucketName,
            DisplayName = snapshot.DisplayName,
            LocalFilePath = snapshot.LocalFilePath,
            Key = snapshot.Key,
            SourcePrefix = snapshot.SourcePrefix,
            LocalTargetPath = snapshot.LocalTargetPath,
            TotalBytes = snapshot.TotalBytes,
            TransferredBytes = snapshot.TransferredBytes,
            Status = TransferStatus.Paused
        };
        task.UploadId = snapshot.UploadId;
        foreach (var part in snapshot.UploadedParts)
            task.UploadedParts.Add(new PartETag(part.PartNumber, part.ETag));
        task.DownloadFiles = snapshot.DownloadFiles?
            .Select(f => new KeyValuePair<string, long>(f.Key, f.Size)).ToList();
        task.DownloadFileIndex = snapshot.DownloadFileIndex;
        task.CurrentFileOffset = snapshot.CurrentFileOffset;
        task.CurrentLocalPath = snapshot.CurrentLocalPath;
        return task;
    }
}
