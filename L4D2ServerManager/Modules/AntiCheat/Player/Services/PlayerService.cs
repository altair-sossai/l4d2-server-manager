using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;
using L4D2ServerManager.Modules.Steam.Services;

namespace L4D2ServerManager.Modules.AntiCheat.Player.Services;

public class PlayerService : IPlayerService
{
	private readonly ISteamService _steamService;

	public PlayerService(ISteamService steamService)
	{
		_steamService = steamService;
	}

	public IPlayer Find(long communityId)
	{
		var player = new Player();

		var playersInfo = _steamService.GetPlayerSummariesAsync(communityId).Result;
		player.Update(playersInfo);

		var gamesInfo = _steamService.GetOwnedGamesAsync(communityId).Result;
		player.Update(gamesInfo);

		return player;
	}
}