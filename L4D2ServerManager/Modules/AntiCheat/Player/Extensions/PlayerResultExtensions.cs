using L4D2ServerManager.Contexts.Steam.Extensions;
using L4D2ServerManager.Contexts.Steam.ValueObjects;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;

namespace L4D2ServerManager.Modules.AntiCheat.Player.Extensions;

public static class PlayerResultExtensions
{
    public static void Update(this PlayerResult playerResult, PlayersInfo? playersInfo)
    {
        var playerInfo = playersInfo.FirstPlayerOrDefault();

        if (playerInfo == null)
            return;

        playerResult.Update(playerInfo);
    }

    private static void Update(this PlayerResult playerResult, PlayerInfo? playerInfo)
    {
        if (playerInfo == null)
            return;

        playerResult.CommunityId = playerInfo.SteamId;
        playerResult.Name = playerInfo.PersonaName;
        playerResult.PictureUrl = playerInfo.AvatarFull;
        playerResult.ProfileUrl = playerInfo.ProfileUrl;
    }

    public static void Update(this PlayerResult playerResult, GamesInfo? gamesInfo)
    {
        var gameInfo = gamesInfo.Left4Dead2();

        if (gameInfo == null)
            return;

        playerResult.Update(gameInfo);
    }

    private static void Update(this PlayerResult playerResult, GameInfo? gameInfo)
    {
        playerResult.TotalHoursPlayed = (gameInfo?.PlayTimeForever ?? 0) / 60;
    }
}