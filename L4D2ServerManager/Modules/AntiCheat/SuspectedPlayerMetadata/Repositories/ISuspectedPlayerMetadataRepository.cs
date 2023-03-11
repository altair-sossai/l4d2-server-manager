using Azure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Repositories;

public interface ISuspectedPlayerMetadataRepository
{
    Pageable<SuspectedPlayerMetadata> GetAllMetadatas(long communityId);
    void AddOrUpdate(IEnumerable<SuspectedPlayerMetadata> metadatas);
    void Delete(long communityId);
    void DeleteOldMetadatas();
}