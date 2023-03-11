using Azure;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Repositories;

public class SuspectedPlayerMetadataRepository : BaseTableStorageRepository<SuspectedPlayerMetadata>, ISuspectedPlayerMetadataRepository
{
    public SuspectedPlayerMetadataRepository(IAzureTableStorageContext tableContext)
        : base("SuspectedPlayerMetadata", tableContext)
    {
    }

    public Pageable<SuspectedPlayerMetadata> GetAllMetadatas(long communityId)
    {
        return TableClient.Query<SuspectedPlayerMetadata>(q => q.PartitionKey == communityId.ToString());
    }

    public void Delete(long communityId)
    {
        foreach (var metadata in GetAllMetadatas(communityId))
            Delete(metadata.PartitionKey, metadata.RowKey);
    }

    public void DeleteOldMetadatas()
    {
        var limit = DateTimeOffset.UtcNow.AddDays(-3);

        foreach (var metadata in TableClient.Query<SuspectedPlayerMetadata>(q => q.Timestamp < limit))
            Delete(metadata.PartitionKey, metadata.RowKey);
    }
}