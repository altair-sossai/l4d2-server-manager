using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing;

public class SuspectedPlayerPing : ITableEntity
{
    public long CommunityId
    {
        get => long.TryParse(RowKey, out var steamId) ? steamId : 0;
        set => RowKey = value.ToString();
    }

    public bool Focused { get; set; }
    public string PartitionKey { get; set; } = "shared";
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}