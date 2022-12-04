using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

public class SuspectedPlayerSecretRepository : BaseTableStorageRepository<SuspectedPlayerSecret>, ISuspectedPlayerSecretRepository
{
    private const string PartitionKey = "shared";

    public SuspectedPlayerSecretRepository(IAzureTableStorageContext tableContext)
        : base("SuspectedPlayerSecret", tableContext)
    {
    }

    public bool Exists(long communityId)
    {
        return Exists(PartitionKey, communityId.ToString());
    }

    public void Delete(long communityId)
    {
        Delete(PartitionKey, communityId.ToString());
    }
}