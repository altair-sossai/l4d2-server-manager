using L4D2ServerManager.Contexts.Steam.Extensions;
using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

public static class SuspectedPlayerExtensions
{
    public static long Steam3(this SuspectedPlayer suspectedPlayer)
    {
        return suspectedPlayer.SteamId == 0 ? 0 : suspectedPlayer.SteamId - 76561197960265728;
    }

    public static long Steam(this SuspectedPlayer suspectedPlayer)
    {
        return suspectedPlayer.Steam3 == 0 ? 0 : suspectedPlayer.Steam3 / 2;
    }

    public static void Update(this SuspectedPlayer suspectedPlayer, PlayersInfo? playersInfo)
    {
        var playerInfo = playersInfo.FirstPlayerOrDefault();

        if (playerInfo == null)
            return;

        suspectedPlayer.Update(playerInfo);
    }

    private static void Update(this SuspectedPlayer suspectedPlayer, PlayerInfo? playerInfo)
    {
        if (playerInfo == null)
            return;

        suspectedPlayer.SteamId = long.TryParse(playerInfo.SteamId, out var steamId) ? steamId : 0;
        suspectedPlayer.Name = playerInfo.PersonaName;
        suspectedPlayer.PictureUrl = playerInfo.AvatarFull;
        suspectedPlayer.ProfileUrl = playerInfo.ProfileUrl;
    }

    public static void Update(this SuspectedPlayer suspectedPlayer, GamesInfo? gamesInfo)
    {
        var gameInfo = gamesInfo.Left4Dead2();

        if (gameInfo == null)
            return;

        suspectedPlayer.Update(gameInfo);
    }

    private static void Update(this SuspectedPlayer suspectedPlayer, GameInfo? gameInfo)
    {
        suspectedPlayer.TotalHoursPlayed = (gameInfo?.PlayTimeForever ?? 0) / 60;
    }
}