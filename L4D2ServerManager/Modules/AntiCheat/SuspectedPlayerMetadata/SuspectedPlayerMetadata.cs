using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata;

public class SuspectedPlayerMetadata : ITableEntity
{
    public long CommunityId
    {
        get => long.TryParse(PartitionKey, out var communityId) ? communityId : 0;
        set => PartitionKey = value.ToString();
    }

    public string? Name { get; set; }
    public string? Value { get; set; }

    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}