using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.Player.Services;

public class PlayerService : IPlayerService
{
	private readonly ISteamContext _steamContext;
	private readonly ISteamPlayerService _steamPlayerService;
	private readonly ISteamUserService _steamUserService;

	public PlayerService(ISteamContext steamContext,
		ISteamUserService steamUserService,
		ISteamPlayerService steamPlayerService)
	{
		_steamContext = steamContext;
		_steamUserService = steamUserService;
		_steamPlayerService = steamPlayerService;
	}

	public IPlayer Find(long communityId)
	{
		var player = new Player();

		var playerSummariesResponse = _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, communityId.ToString()).Result;
		player.Update(playerSummariesResponse.Response);

		var ownedGamesResponse = _steamPlayerService.GetOwnedGamesAsync(_steamContext.SteamApiKey, communityId.ToString()).Result;
		player.Update(ownedGamesResponse.Response);

		return player;
	}
}