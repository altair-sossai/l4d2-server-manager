using L4D2ServerManager.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

public static class SuspectedPlayerCommandExtensions
{
    public static string? CustomUrl(this SuspectedPlayerCommand command)
    {
        var patterns = new[]
        {
            @"(^[^\/ ]+$)",
            @"https?:\/\/steamcommunity.com\/id\/([^\/ ]+)/?$"
        };

        return command.Account?.MatchValue(patterns);
    }
}