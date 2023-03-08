using Azure;
using Azure.Data.Tables;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Enums;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail;

public class SuspectedPlayerFileFail : ITableEntity
{
    public long CommunityId
    {
        get => long.TryParse(PartitionKey, out var communityId) ? communityId : 0;
        set => PartitionKey = value.ToString();
    }

    public string? File { get; set; }
    public FailReason Reason { get; set; }
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}