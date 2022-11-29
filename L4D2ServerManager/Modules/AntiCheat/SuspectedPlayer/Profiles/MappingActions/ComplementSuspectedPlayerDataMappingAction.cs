using AutoMapper;
using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Profiles.MappingActions;

public class ComplementSuspectedPlayerDataMappingAction : IMappingAction<SuspectedPlayerCommand, SuspectedPlayer>
{
    private readonly IPlayerService _playerService;
    private readonly ISteamContext _steamContext;
    private readonly ISteamUserService _steamUserService;

    public ComplementSuspectedPlayerDataMappingAction(ISteamContext steamContext,
        ISteamUserService steamUserService,
        IPlayerService playerService)
    {
        _steamContext = steamContext;
        _steamUserService = steamUserService;
        _playerService = playerService;
    }

    public void Process(SuspectedPlayerCommand command, SuspectedPlayer suspectedPlayer, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(suspectedPlayer.SteamId))
            suspectedPlayer.SteamId = ResolveSteamIdAsync(command.Login).Result ?? command.SteamId;

        if (string.IsNullOrEmpty(suspectedPlayer.SteamId))
            return;

        var playerSummariesResponse = _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, suspectedPlayer.SteamId).Result;
        suspectedPlayer.Update(playerSummariesResponse.Response);

        var ownedGamesResponse = _playerService.GetOwnedGamesAsync(_steamContext.SteamApiKey, suspectedPlayer.SteamId).Result;
        suspectedPlayer.Update(ownedGamesResponse.Response);
    }

    private async Task<string?> ResolveSteamIdAsync(string? login)
    {
        if (string.IsNullOrEmpty(login))
            return await Task.FromResult((string?)null);

        var response = await _steamUserService.ResolveVanityUrlAsync(_steamContext.SteamApiKey, login);

        return response is { Response: { Success: 1 } } ? response.Response.SteamId : null;
    }
}