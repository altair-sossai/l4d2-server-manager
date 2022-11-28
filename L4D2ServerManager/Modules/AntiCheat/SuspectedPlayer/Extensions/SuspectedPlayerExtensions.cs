using L4D2ServerManager.Contexts.Steam.Extensions;
using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

public static class SuspectedPlayerExtensions
{
    public static void Update(this SuspectedPlayer suspectedPlayer, PlayersInfo playersInfo)
    {
        var playerInfo = playersInfo.FirstPlayerOrDefault();

        if (playerInfo == null)
            return;

        suspectedPlayer.Update(playerInfo);
    }

    private static void Update(this SuspectedPlayer suspectedPlayer, PlayerInfo playerInfo)
    {
        suspectedPlayer.SteamId = playerInfo.SteamId;
        suspectedPlayer.Name = playerInfo.PersonaName;
        suspectedPlayer.PictureUrl = playerInfo.AvatarFull;
        suspectedPlayer.ProfileUrl = playerInfo.ProfileUrl;
    }

    public static void Update(this SuspectedPlayer suspectedPlayer, GamesInfo gamesInfo)
    {
        var gameInfo = gamesInfo.Left4Dead2();

        if (gameInfo == null)
            return;

        suspectedPlayer.Update(gameInfo);
    }

    private static void Update(this SuspectedPlayer suspectedPlayer, GameInfo gameInfo)
    {
        suspectedPlayer.TotalHoursPlayed = gameInfo.PlayTimeForever / 60;
    }
}