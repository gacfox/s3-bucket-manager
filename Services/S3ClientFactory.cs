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
                new AmazonS3Config { ServiceURL = endpoint, ForcePathStyle = true });
        }
    }
}
