using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Services;

public interface ISuspectedPlayerFileFailService
{
    void BatchOperation(long communityId, IEnumerable<SuspectedPlayerFileFailCommand> commands);
}