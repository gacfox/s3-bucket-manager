namespace Gacfox.S3BucketManager.Models;

public class ConnectionProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public bool UseSsl { get; set; } = true;
}

public class ConnectionCredentials
{
    public Guid ConnectionId { get; set; }
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
}
