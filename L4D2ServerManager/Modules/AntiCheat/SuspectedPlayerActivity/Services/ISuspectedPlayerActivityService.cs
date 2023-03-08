using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Results;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Services;

public interface ISuspectedPlayerActivityService
{
    CheckAntiCheatUsageResult CheckAntiCheatUsage(CheckAntiCheatUsageCommand command);
}