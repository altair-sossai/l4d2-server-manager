using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer;

public class SuspectedPlayer : ITableEntity
{
    public string? SteamId
    {
        get => RowKey;
        set => RowKey = value ?? null!;
    }

    public string? Name { get; set; }
    public string? PictureUrl { get; set; }
    public string? ProfileUrl { get; set; }
    public int TotalHoursPlayed { get; set; }

    public static SuspectedPlayer Default => new();

    public string PartitionKey { get; set; } = "shared";
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}