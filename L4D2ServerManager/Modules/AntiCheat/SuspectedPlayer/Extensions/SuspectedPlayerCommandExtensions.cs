using L4D2ServerManager.Contexts.Steam.Helpers;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

public static class SuspectedPlayerCommandExtensions
{
    public static string? CustomUrl(this SuspectedPlayerCommand command)
    {
        return SteamIdHelper.CustomUrl(command.Account ?? string.Empty);
    }
}