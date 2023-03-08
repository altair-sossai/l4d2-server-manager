using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess;

public class SuspectedPlayerProcess : ITableEntity
{
    public long CommunityId
    {
        get => long.TryParse(PartitionKey, out var communityId) ? communityId : 0;
        set => PartitionKey = value.ToString();
    }

    public string? ProcessName { get; set; }
    public string? WindowTitle { get; set; }
    public string? FileName { get; set; }
    public string? Module { get; set; }
    public string? CompanyName { get; set; }
    public string? FileDescription { get; set; }
    public string? FileVersion { get; set; }
    public string? OriginalFilename { get; set; }
    public string? ProductName { get; set; }

    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}