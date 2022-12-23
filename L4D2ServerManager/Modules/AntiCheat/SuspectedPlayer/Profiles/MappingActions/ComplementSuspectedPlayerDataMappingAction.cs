using AutoMapper;
using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.Player.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Profiles.MappingActions;

public class ComplementSuspectedPlayerDataMappingAction : IMappingAction<SuspectedPlayerCommand, SuspectedPlayer>
{
	private readonly ISteamContext _steamContext;
	private readonly ISteamIdService _steamIdService;
	private readonly ISteamPlayerService _steamPlayerService;
	private readonly ISteamUserService _steamUserService;

	public ComplementSuspectedPlayerDataMappingAction(ISteamContext steamContext,
		ISteamUserService steamUserService,
		ISteamIdService steamIdService,
		ISteamPlayerService steamPlayerService)
	{
		_steamContext = steamContext;
		_steamUserService = steamUserService;
		_steamIdService = steamIdService;
		_steamPlayerService = steamPlayerService;
	}

	public void Process(SuspectedPlayerCommand command, SuspectedPlayer suspectedPlayer, ResolutionContext context)
	{
		if (suspectedPlayer.CommunityId == 0)
			suspectedPlayer.CommunityId = _steamIdService.ResolveSteamIdAsync(command.CustomUrl).Result ?? command.CommunityId ?? 0;

		if (suspectedPlayer.CommunityId == 0)
			return;

		var playerSummariesResponse = _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, suspectedPlayer.CommunityId.ToString()).Result;
		suspectedPlayer.Update(playerSummariesResponse.Response);

		var ownedGamesResponse = _steamPlayerService.GetOwnedGamesAsync(_steamContext.SteamApiKey, suspectedPlayer.CommunityId.ToString()).Result;
		suspectedPlayer.Update(ownedGamesResponse.Response);
	}
}