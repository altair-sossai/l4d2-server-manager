using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Services;

public interface ISuspectedPlayerServiceSecret
{
    SuspectedPlayerSecret Add(AddSuspectedPlayerSecretCommand command);
}