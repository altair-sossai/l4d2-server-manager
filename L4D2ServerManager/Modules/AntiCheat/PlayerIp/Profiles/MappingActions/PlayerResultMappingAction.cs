using AutoMapper;
using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;
using L4D2ServerManager.Modules.Steam.Services;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Profiles.MappingActions;

public class PlayerResultMappingAction : IMappingAction<PlayerIp, PlayerResult>
{
	private readonly ISteamContext _steamContext;
	private readonly ISteamService _steamService;
	private readonly ISteamUserService _steamUserService;

	public PlayerResultMappingAction(ISteamService steamService,
		ISteamContext steamContext,
		ISteamUserService steamUserService)
	{
		_steamService = steamService;
		_steamContext = steamContext;
		_steamUserService = steamUserService;
	}


	public void Process(PlayerIp playerIp, PlayerResult result, ResolutionContext context)
	{
		var playerSummariesResponse = _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, playerIp.CommunityId.ToString()).Result;
		result.Update(playerSummariesResponse.Response);

		var gamesInfo = _steamService.GetOwnedGamesAsync(playerIp.CommunityId).Result;
		result.Update(gamesInfo);
	}
}