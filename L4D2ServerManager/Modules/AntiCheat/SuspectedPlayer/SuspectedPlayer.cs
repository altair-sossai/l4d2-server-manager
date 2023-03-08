using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer;

public class SuspectedPlayer : Player.Player, ITableEntity
{
    public static SuspectedPlayer Default => new();

    public string PartitionKey { get; set; } = "shared";

    public string RowKey
    {
        get => CommunityId.ToString();
        set => CommunityId = long.TryParse(value, out var communityId) ? communityId : 0;
    }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}