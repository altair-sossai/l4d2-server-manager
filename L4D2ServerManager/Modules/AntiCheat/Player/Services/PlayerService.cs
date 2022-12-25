using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;
using L4D2ServerManager.Modules.Steam.Services;

namespace L4D2ServerManager.Modules.AntiCheat.Player.Services;

public class PlayerService : IPlayerService
{
	private readonly ISteamContext _steamContext;
	private readonly ISteamService _steamService;
	private readonly ISteamUserService _steamUserService;

	public PlayerService(ISteamService steamService,
		ISteamContext steamContext,
		ISteamUserService steamUserService)
	{
		_steamService = steamService;
		_steamContext = steamContext;
		_steamUserService = steamUserService;
	}

	public IPlayer Find(long communityId)
	{
		var player = new Player();

		var playerSummariesResponse = _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, communityId.ToString()).Result;
		player.Update(playerSummariesResponse.Response);

		var gamesInfo = _steamService.GetOwnedGamesAsync(communityId).Result;
		player.Update(gamesInfo);

		return player;
	}
}