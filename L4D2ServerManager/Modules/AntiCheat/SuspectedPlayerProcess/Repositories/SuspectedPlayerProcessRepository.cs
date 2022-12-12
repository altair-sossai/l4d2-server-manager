using Azure;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Repositories;

public class SuspectedPlayerProcessRepository : BaseTableStorageRepository<SuspectedPlayerProcess>, ISuspectedPlayerProcessRepository
{
	public SuspectedPlayerProcessRepository(IAzureTableStorageContext tableContext)
		: base("SuspectedPlayerProcess", tableContext)
	{
	}

	public Pageable<SuspectedPlayerProcess> GetAllProcesses(long communityId)
	{
		return TableClient.Query<SuspectedPlayerProcess>(q => q.PartitionKey == communityId.ToString());
	}
}