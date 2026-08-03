using System.Text.Json;
using Gacfox.S3BucketManager.Models;

namespace Gacfox.S3BucketManager.Services;

public static class TransferStore
{
    private static readonly string StorePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "s3BucketManager", "transfers.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static List<TransferTaskSnapshot> Load()
    {
        if (!File.Exists(StorePath)) return new List<TransferTaskSnapshot>();
        try
        {
            return JsonSerializer.Deserialize<List<TransferTaskSnapshot>>(
                File.ReadAllText(StorePath), JsonOptions) ?? new List<TransferTaskSnapshot>();
        }
        catch (JsonException)
        {
            return new List<TransferTaskSnapshot>();
        }
    }

    public static void Save(IEnumerable<TransferTaskSnapshot> snapshots)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StorePath)!);
        File.WriteAllText(StorePath, JsonSerializer.Serialize(snapshots, JsonOptions));
    }
}
