using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Services;

public interface ISuspectedPlayerMetadataService
{
    void BatchOperation(long communityId, IEnumerable<MetadataCommand> commands);
}