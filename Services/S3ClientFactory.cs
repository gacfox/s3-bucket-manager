using Amazon.Runtime;
using Amazon.S3;
using Gacfox.S3BucketManager.Models;

namespace Gacfox.S3BucketManager.Services
{
    public static class S3ClientFactory
    {
        public static AmazonS3Client Create(ConnectionProfile profile, ConnectionCredentials credentials)
        {
            var endpoint = profile.Endpoint.Trim();
            if (!endpoint.Contains("://"))
                endpoint = (profile.UseSsl ? "https://" : "http://") + endpoint;
            return new AmazonS3Client(
                new BasicAWSCredentials(credentials.AccessKey, credentials.SecretKey),
                new AmazonS3Config
                {
                    ServiceURL = endpoint,
                    ForcePathStyle = true,
                    // v4 默认的 WHEN_SUPPORTED 校验会让部分 S3 兼容存储报 content-sha256 不匹配
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
                });
        }
    }
}
