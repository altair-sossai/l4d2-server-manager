using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Repositories;

public class PlayerIpRepository : BaseTableStorageRepository<PlayerIp>, IPlayerIpRepository
{
	public PlayerIpRepository(IAzureTableStorageContext tableContext)
		: base("PlayerIP", tableContext)
	{
	}

	public List<PlayerIp> GetAllPlayerIps(long communityId)
	{
		return TableClient
			.Query<PlayerIp>(q => q.RowKey == communityId.ToString())
			.OrderByDescending(w => w.When)
			.ToList();
	}

	public List<PlayerIp> GetAllPlayersWithIp(string ip)
	{
		return TableClient
			.Query<PlayerIp>(q => q.PartitionKey == ip)
			.OrderByDescending(w => w.When)
			.ToList();
	}

	public List<PlayerIp> GetAllPlayersWithIp(string ip, long ignore)
	{
		return TableClient
			.Query<PlayerIp>(q => q.PartitionKey == ip && q.RowKey != ignore.ToString())
			.OrderByDescending(w => w.When)
			.ToList();
	}

	public void Delete(long communityId)
	{
		foreach (var playerIp in TableClient.Query<PlayerIp>(q => q.RowKey == communityId.ToString()))
			Delete(playerIp.PartitionKey, playerIp.RowKey);
	}

	public void DeleteOldIps()
	{
		var limit = DateTimeOffset.UtcNow.AddDays(-10);

		foreach (var playerIp in TableClient.Query<PlayerIp>(q => q.Timestamp < limit))
			Delete(playerIp.PartitionKey, playerIp.RowKey);
	}
}