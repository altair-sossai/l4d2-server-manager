using Azure;
using Azure.Data.Tables;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer;

public class SuspectedPlayer : ITableEntity
{
    public long SteamId
    {
        get => long.TryParse(RowKey, out var steamId) ? steamId : 0;
        set => RowKey = value.ToString();
    }

    public long Steam3 => this.Steam3();
    public long Steam => this.Steam();
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