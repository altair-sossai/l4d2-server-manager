using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

public abstract class Activity : ITableEntity
{
	protected Activity(long communityId)
	{
		CommunityId = communityId;
	}

	public long CommunityId
	{
		get => long.TryParse(RowKey, out var communityId) ? communityId : 0;
		set => RowKey = value.ToString();
	}

	public string PartitionKey { get; set; } = "shared";
	public string RowKey { get; set; } = default!;
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}