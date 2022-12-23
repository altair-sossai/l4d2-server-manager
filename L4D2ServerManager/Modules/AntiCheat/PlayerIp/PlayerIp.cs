using Azure;
using Azure.Data.Tables;
using L4D2ServerManager.Contexts.Steam.Structures;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp;

public class PlayerIp : ITableEntity
{
	private SteamIdentifiers _steamIdentifiers;

	public string Ip
	{
		get => PartitionKey;
		set => PartitionKey = value;
	}

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
	public DateTime When { get; set; } = DateTime.UtcNow;

	public string PartitionKey { get; set; } = default!;
	public string RowKey { get; set; } = default!;
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}