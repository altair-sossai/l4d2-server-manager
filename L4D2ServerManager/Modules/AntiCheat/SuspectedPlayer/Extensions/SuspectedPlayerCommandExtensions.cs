using System.Text.RegularExpressions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

public static class SuspectedPlayerCommandExtensions
{
    public static string? Login(this SuspectedPlayerCommand command)
    {
        var patterns = new[]
        {
            @"(^[^\/ ]+$)",
            @"https?:\/\/steamcommunity.com\/id\/([^\/ ]+)/?$"
        };

        return command.MatchValue(patterns);
    }

    public static long? SteamId(this SuspectedPlayerCommand command)
    {
        var patterns = new[]
        {
            @"(^\d+$)",
            @"^https?:\/\/steamcommunity\.com\/profiles\/(\d+)\/?$"
        };

        return long.TryParse(command.MatchValue(patterns), out var steamId) ? steamId : null;
    }

    private static string? MatchValue(this SuspectedPlayerCommand command, IEnumerable<string> patterns)
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