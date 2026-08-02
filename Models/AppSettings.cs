namespace Gacfox.S3BucketManager.Models;

public class AppSettings
{
    public int UploadConcurrency { get; set; } = 3;
    public int DownloadConcurrency { get; set; } = 3;
    public int LinkExpirationSeconds { get; set; } = 3600;
}
