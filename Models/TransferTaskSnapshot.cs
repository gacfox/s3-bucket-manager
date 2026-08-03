namespace Gacfox.S3BucketManager.Models;

public class TransferTaskSnapshot
{
    public Guid Id { get; set; }
    public int Direction { get; set; }
    public Guid ConnectionId { get; set; }
    public string BucketName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public long TotalBytes { get; set; }
    public long TransferredBytes { get; set; }
    public string? LocalFilePath { get; set; }
    public string? Key { get; set; }
    public string? SourcePrefix { get; set; }
    public string? LocalTargetPath { get; set; }
    public string? UploadId { get; set; }
    public List<UploadPartSnapshot> UploadedParts { get; set; } = new();
    public List<DownloadFileSnapshot>? DownloadFiles { get; set; }
    public int DownloadFileIndex { get; set; }
    public long CurrentFileOffset { get; set; }
    public string? CurrentLocalPath { get; set; }
}

public class UploadPartSnapshot
{
    public int PartNumber { get; set; }
    public string ETag { get; set; } = "";
}

public class DownloadFileSnapshot
{
    public string Key { get; set; } = "";
    public long Size { get; set; }
}
