using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Gacfox.S3BucketManager.Models;

namespace Gacfox.S3BucketManager.Services;

public class ConnectionStore
{
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "s3BucketManager");
    private static readonly string ConfigPath = Path.Combine(DataDirectory, "configuration.json");
    private static readonly string VaultPath = Path.Combine(DataDirectory, "accounts.vault");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public List<ConnectionProfile> Connections { get; } = new();

    public AppSettings Settings { get; private set; } = new();

    private readonly List<ConnectionCredentials> _credentials = new();

    public static ConnectionStore Load()
    {
        var store = new ConnectionStore();
        if (File.Exists(ConfigPath))
        {
            var data = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(ConfigPath), JsonOptions);
            if (data?.Connections != null)
                store.Connections.AddRange(data.Connections);
            if (data?.Settings != null)
                store.Settings = data.Settings;
        }
        if (File.Exists(VaultPath))
        {
            try
            {
                var bytes = ProtectedData.Unprotect(
                    Convert.FromBase64String(File.ReadAllText(VaultPath)), null, DataProtectionScope.CurrentUser);
                var credentials = JsonSerializer.Deserialize<List<ConnectionCredentials>>(
                    Encoding.UTF8.GetString(bytes), JsonOptions);
                if (credentials != null)
                    store._credentials.AddRange(credentials);
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException or JsonException)
            {
                // vault 损坏或由其他 Windows 用户加密时按无凭据处理
            }
        }
        return store;
    }

    public ConnectionCredentials? GetCredentials(Guid connectionId)
        => _credentials.FirstOrDefault(c => c.ConnectionId == connectionId);

    public void Add(ConnectionProfile profile, ConnectionCredentials credentials)
    {
        credentials.ConnectionId = profile.Id;
        Connections.Add(profile);
        _credentials.Add(credentials);
        Save();
    }

    public void SaveSettings() => Save();

    public void Remove(Guid connectionId)
    {
        Connections.RemoveAll(c => c.Id == connectionId);
        _credentials.RemoveAll(c => c.ConnectionId == connectionId);
        Save();
    }

    private void Save()
    {
        Directory.CreateDirectory(DataDirectory);
        File.WriteAllText(ConfigPath, JsonSerializer.Serialize(
            new ConfigData { Connections = Connections, Settings = Settings }, JsonOptions));
        var protectedBytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_credentials, JsonOptions)), null, DataProtectionScope.CurrentUser);
        File.WriteAllText(VaultPath, Convert.ToBase64String(protectedBytes));
    }

    private class ConfigData
    {
        public List<ConnectionProfile> Connections { get; set; } = new();
        public AppSettings Settings { get; set; } = new();
    }
}
