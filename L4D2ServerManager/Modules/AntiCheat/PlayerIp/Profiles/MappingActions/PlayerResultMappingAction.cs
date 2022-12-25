using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;
using L4D2ServerManager.Modules.Steam.Services;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Profiles.MappingActions;

public class PlayerResultMappingAction : IMappingAction<PlayerIp, PlayerResult>
{
	private readonly ISteamService _steamService;

	public PlayerResultMappingAction(ISteamService steamService)
	{
		_steamService = steamService;
	}


	public void Process(PlayerIp playerIp, PlayerResult result, ResolutionContext context)
	{
		var playersInfo = _steamService.GetPlayerSummariesAsync(playerIp.CommunityId).Result;
		result.Update(playersInfo);

		var gamesInfo = _steamService.GetOwnedGamesAsync(playerIp.CommunityId).Result;
		result.Update(gamesInfo);
	}
}