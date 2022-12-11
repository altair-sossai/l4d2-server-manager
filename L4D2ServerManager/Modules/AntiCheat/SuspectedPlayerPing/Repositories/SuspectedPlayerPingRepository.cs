using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;

public class SuspectedPlayerPingRepository : BaseTableStorageRepository<SuspectedPlayerPing>, ISuspectedPlayerPingRepository
{
    public SuspectedPlayerPingRepository(IAzureTableStorageContext tableContext)
        : base("SuspectedPlayerPing", tableContext)
    {
    }
}