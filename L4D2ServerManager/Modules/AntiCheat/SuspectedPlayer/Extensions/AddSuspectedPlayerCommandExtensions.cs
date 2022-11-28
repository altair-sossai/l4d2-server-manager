using System.Text.RegularExpressions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

public static class AddSuspectedPlayerCommandExtensions
{
    public static string? Login(this AddSuspectedPlayerCommand command)
    {
        var patterns = new[]
        {
            @"(^[^\/ ]+$)",
            @"https?:\/\/steamcommunity.com\/id\/([^\/ ]+)/?$"
        };

        return command.MatchValue(patterns);
    }

    public static string? SteamId(this AddSuspectedPlayerCommand command)
    {
        var patterns = new[]
        {
            @"(^\d+$)",
            @"^https?:\/\/steamcommunity\.com\/profiles\/(\d+)\/?$"
        };

        return command.MatchValue(patterns);
    }

    private static string? MatchValue(this AddSuspectedPlayerCommand command, IEnumerable<string> patterns)
    {
        if (string.IsNullOrEmpty(command.SuspectedPlayer))
            return null;

        var pattern = patterns.FirstOrDefault(pattern => Regex.IsMatch(command.SuspectedPlayer, pattern));

        if (string.IsNullOrEmpty(pattern))
            return null;

        var match = Regex.Match(command.SuspectedPlayer, pattern);
        var group = match.Groups[1];

        return group.Value;
    }
}