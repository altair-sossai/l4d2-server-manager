using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;

public class SuspectedPlayerRepository : BaseTableStorageRepository<SuspectedPlayer>, ISuspectedPlayerRepository
{
    private const string PartitionKey = "shared";

    public SuspectedPlayerRepository(IAzureTableStorageContext tableContext)
        : base("SuspectedPlayer", tableContext)
    {
    }

    public bool Exists(long communityId)
    {
        return Exists(PartitionKey, communityId.ToString());
    }

    public SuspectedPlayer? GetSuspectedPlayer(long communityId)
    {
        return communityId == 0 ? null : Find(PartitionKey, communityId.ToString());
    }

    public IEnumerable<SuspectedPlayer> GetSuspectedPlayers()
    {
        return GetAll().OrderBy(o => o.Name);
    }

    public void Delete(long communityId)
    {
        Delete(PartitionKey, communityId.ToString());
    }
}