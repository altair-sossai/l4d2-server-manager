using Azure;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Repositories;

public class SuspectedPlayerFileFailRepository : BaseTableStorageRepository<SuspectedPlayerFileFail>, ISuspectedPlayerFileFailRepository
{
	public SuspectedPlayerFileFailRepository(IAzureTableStorageContext tableContext)
		: base("SuspectedPlayerFileFail", tableContext)
	{
	}

	public Pageable<SuspectedPlayerFileFail> GetAllFiles(long communityId)
	{
		return TableClient.Query<SuspectedPlayerFileFail>(q => q.PartitionKey == communityId.ToString());
	}

	public void Delete(long communityId)
	{
		foreach (var file in GetAllFiles(communityId))
			Delete(file.PartitionKey, file.RowKey);
	}

	public void DeleteOldFiles()
	{
		var limit = DateTimeOffset.UtcNow.AddDays(-3);

		foreach (var file in TableClient.Query<SuspectedPlayerFileFail>(q => q.Timestamp < limit))
			Delete(file.PartitionKey, file.RowKey);
	}
}