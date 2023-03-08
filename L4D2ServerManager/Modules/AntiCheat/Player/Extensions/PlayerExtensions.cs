using L4D2ServerManager.Contexts.Steam.Extensions;
using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Modules.AntiCheat.Player.Extensions;

public static class PlayerExtensions
{
    public static void Update(this IPlayer player, PlayersInfo? playersInfo)
    {
        var playerInfo = playersInfo.FirstPlayerOrDefault();

        if (playerInfo == null)
            return;

        player.Update(playerInfo);
    }

    private static void Update(this IPlayer player, PlayerInfo? playerInfo)
    {
        if (playerInfo == null)
            return;

        player.CommunityId = long.TryParse(playerInfo.SteamId, out var communityId) ? communityId : 0;
        player.Name = playerInfo.PersonaName;
        player.PictureUrl = playerInfo.AvatarFull;
        player.ProfileUrl = playerInfo.ProfileUrl;
    }

    public static void Update(this IPlayer player, GamesInfo? gamesInfo)
    {
        var gameInfo = gamesInfo.Left4Dead2();

        if (gameInfo == null)
            return;

        player.Update(gameInfo);
    }

    private static void Update(this IPlayer player, GameInfo? gameInfo)
    {
        player.TotalHoursPlayed = (gameInfo?.PlayTimeForever ?? 0) / 60;
    }
}