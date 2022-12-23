using AutoMapper;
using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Profiles.MappingActions;

public class PlayerResultMappingAction : IMappingAction<PlayerIp, PlayerResult>
{
	private readonly IPlayerService _playerService;
	private readonly ISteamContext _steamContext;
	private readonly ISteamUserService _steamUserService;

	public PlayerResultMappingAction(ISteamContext steamContext,
		ISteamUserService steamUserService,
		IPlayerService playerService)
	{
		_steamContext = steamContext;
		_steamUserService = steamUserService;
		_playerService = playerService;
	}


	public void Process(PlayerIp playerIp, PlayerResult result, ResolutionContext context)
	{
		var playerSummariesResponse = _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, playerIp.CommunityId.ToString()).Result;
		result.Update(playerSummariesResponse.Response);

		var ownedGamesResponse = _playerService.GetOwnedGamesAsync(_steamContext.SteamApiKey, playerIp.CommunityId.ToString()).Result;
		result.Update(ownedGamesResponse.Response);
	}
}