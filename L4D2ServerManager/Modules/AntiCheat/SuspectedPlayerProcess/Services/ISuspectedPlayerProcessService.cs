using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Services;

public interface ISuspectedPlayerProcessService
{
    void BatchOperation(long communityId, IEnumerable<ProcessCommand> commands);
}