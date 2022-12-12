using Azure;
using Azure.Data.Tables;
using L4D2ServerManager.Infrastructure.Helpers;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret;

public class SuspectedPlayerSecret : ITableEntity
{
	public long CommunityId
	{
		get => long.TryParse(RowKey, out var communityId) ? communityId : 0;
		set => RowKey = value.ToString();
	}

	public string? Secret { get; set; } = StringHelper.RandomString(40);
	public string PartitionKey { get; set; } = "shared";
	public string RowKey { get; set; } = default!;
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}