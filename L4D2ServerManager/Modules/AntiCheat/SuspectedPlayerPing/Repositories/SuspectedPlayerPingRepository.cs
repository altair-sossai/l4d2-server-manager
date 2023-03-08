using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;

public class SuspectedPlayerPingRepository : BaseTableStorageRepository<SuspectedPlayerPing>, ISuspectedPlayerPingRepository
{
    public SuspectedPlayerPingRepository(IAzureTableStorageContext tableContext)
        : base("SuspectedPlayerPing", tableContext)
    {
    }

    public SuspectedPlayerPing? Find(long communityId)
    {
        return Find("shared", communityId.ToString());
    }

    public void Delete(long communityId)
    {
        Delete("shared", communityId.ToString());
    }
}