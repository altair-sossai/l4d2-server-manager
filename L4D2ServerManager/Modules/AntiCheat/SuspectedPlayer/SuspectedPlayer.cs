using Azure;
using Azure.Data.Tables;
using L4D2ServerManager.Contexts.Steam.Structures;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer;

public class SuspectedPlayer : ITableEntity
{
    private SteamIdentifiers _steamIdentifiers;

    public long CommunityId
    {
        get => long.TryParse(RowKey, out var communityId) ? communityId : 0;
        set
        {
            RowKey = value.ToString();
            SteamIdentifiers.TryParse(RowKey, out _steamIdentifiers);
        }
    }

    public string? SteamId => _steamIdentifiers.SteamId;
    public string? Steam3 => _steamIdentifiers.Steam3;

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