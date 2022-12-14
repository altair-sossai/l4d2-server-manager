using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity;

public class SuspectedPlayerActivity : ITableEntity
{
	public long CommunityId
	{
		get => long.TryParse(RowKey, out var communityId) ? communityId : 0;
		set => RowKey = value.ToString();
	}

	public DateTime? PingFocused { get; set; }
	public DateTime? PingUnfocused { get; set; }
	public DateTime? Process { get; set; }
	public DateTime? Screenshot { get; set; }

	public string PartitionKey { get; set; } = "shared";
	public string RowKey { get; set; } = default!;
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}