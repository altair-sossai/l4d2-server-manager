using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.ServerPing.Repositories;

public class ServerPingRepository : BaseTableStorageRepository<ServerPing>, IServerPingRepository
{
	public ServerPingRepository(IAzureTableStorageContext tableContext)
		: base("ServerPing", tableContext)
	{
	}

	public ServerPing? Get()
	{
		return Find("shared", "shared");
	}
}