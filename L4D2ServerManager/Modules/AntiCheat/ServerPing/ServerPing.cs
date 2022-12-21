using Azure;
using Azure.Data.Tables;

namespace L4D2ServerManager.Modules.AntiCheat.ServerPing;

public class ServerPing : ITableEntity
{
	public DateTime When { get; set; } = DateTime.UtcNow;
	public string PartitionKey { get; set; } = "shared";
	public string RowKey { get; set; } = "shared";
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}