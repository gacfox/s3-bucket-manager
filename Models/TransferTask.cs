using Amazon.S3.Model;

namespace Gacfox.S3BucketManager.Models
{
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
    }
}
