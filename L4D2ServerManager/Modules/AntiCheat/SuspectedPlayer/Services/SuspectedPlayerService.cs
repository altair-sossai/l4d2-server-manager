using Azure.Data.Tables;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public class SuspectedPlayerService : ISuspectedPlayerService
{
    private static bool _created;
    private readonly IPlayerService _playerService;
    private readonly SteamContext _steamContext;
    private readonly ISteamUserService _steamUserService;
    private readonly IAzureTableStorageContext _tableContext;
    private TableClient? _userTable;

    public SuspectedPlayerService(IAzureTableStorageContext tableContext,
        SteamContext steamContext,
        IPlayerService playerService,
        ISteamUserService steamUserService)
    {
        _tableContext = tableContext;
        _steamContext = steamContext;
        _playerService = playerService;
        _steamUserService = steamUserService;

        CreateIfNotExists();
    }

    private TableClient SuspectedPlayerTable => _userTable ??= _tableContext.GetTableClient("SuspectedPlayers").Result;

    public IEnumerable<SuspectedPlayer> GetSuspectedPlayers()
    {
        return SuspectedPlayerTable.Query<SuspectedPlayer>();
    }

    public async Task<SuspectedPlayer?> AddAsync(AddSuspectedPlayerCommand command)
    {
        var steamId = await ResolveSteamIdAsync(command.Login) ?? command.SteamId;
        var suspectedPlayer = await BuildSuspectedPlayerAsync(steamId);


        return suspectedPlayer;
    }

    private async Task<string?> ResolveSteamIdAsync(string? login)
    {
        if (string.IsNullOrEmpty(login))
            return await Task.FromResult((string?)null);

        var response = await _steamUserService.ResolveVanityUrlAsync(_steamContext.SteamApiKey, login);

        return response is { Response: { Success: 1 } } ? response.Response.SteamId : null;
    }

    private async Task<SuspectedPlayer?> BuildSuspectedPlayerAsync(string? steamId)
    {
        if (string.IsNullOrEmpty(steamId))
            return await Task.FromResult((SuspectedPlayer?)null);

        var suspectedPlayer = new SuspectedPlayer();

        var playerSummariesResponse = await _steamUserService.GetPlayerSummariesAsync(_steamContext.SteamApiKey, steamId);
        if (playerSummariesResponse.Response != null)
            suspectedPlayer.Update(playerSummariesResponse.Response);

        var ownedGamesResponse = await _playerService.GetOwnedGamesAsync(_steamContext.SteamApiKey, steamId);
        if (ownedGamesResponse.Response != null)
            suspectedPlayer.Update(ownedGamesResponse.Response);

        return suspectedPlayer;
    }

    private void CreateIfNotExists()
    {
        if (_created)
            return;

        SuspectedPlayerTable.CreateIfNotExists();

        _created = true;
    }
}