using AutoMapper;
using Azure.Data.Tables;
using FluentValidation;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public class SuspectedPlayerService : ISuspectedPlayerService
{
    private static bool _created;
    private readonly IMapper _mapper;
    private readonly ISteamContext _steamContext;
    private readonly ISteamUserService _steamUserService;
    private readonly IAzureTableStorageContext _tableContext;
    private readonly IValidator<SuspectedPlayer> _validator;
    private TableClient? _userTable;

    public SuspectedPlayerService(IMapper mapper,
        ISteamContext steamContext,
        ISteamUserService steamUserService,
        IAzureTableStorageContext tableContext,
        IValidator<SuspectedPlayer> validator)
    {
        _mapper = mapper;
        _steamContext = steamContext;
        _steamUserService = steamUserService;
        _tableContext = tableContext;
        _validator = validator;

        CreateIfNotExists();
    }

    private TableClient SuspectedPlayerTable => _userTable ??= _tableContext.GetTableClient("SuspectedPlayers").Result;

    public SuspectedPlayer? GetSuspectedPlayer(long steamId)
    {
        return steamId == 0 ? null : SuspectedPlayerTable.Query<SuspectedPlayer>(q => q.RowKey == steamId.ToString()).FirstOrDefault();
    }

    public IEnumerable<SuspectedPlayer> GetSuspectedPlayers()
    {
        return SuspectedPlayerTable.Query<SuspectedPlayer>().OrderBy(o => o.Name);
    }

    public SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command)
    {
        var steamId = ResolveSteamIdAsync(command.Login).Result ?? command.SteamId ?? 0;
        var suspectedPlayer = GetSuspectedPlayer(steamId) ?? SuspectedPlayer.Default;

        _mapper.Map(command, suspectedPlayer);
        _validator.ValidateAndThrowAsync(suspectedPlayer).Wait();

        SuspectedPlayerTable.UpsertEntity(suspectedPlayer);

        return suspectedPlayer;
    }

    public void Sync()
    {
        foreach (var suspectedPlayer in GetSuspectedPlayers())
        {
            var suspectedPlayerCommand = new SuspectedPlayerCommand
            {
                SuspectedPlayer = suspectedPlayer.SteamId.ToString()
            };

            AddOrUpdate(suspectedPlayerCommand);
        }
    }

    public void Delete(string? steamId)
    {
        SuspectedPlayerTable.DeleteEntity("shared", steamId);
    }

    private void CreateIfNotExists()
    {
        if (_created)
            return;

        SuspectedPlayerTable.CreateIfNotExists();

        _created = true;
    }

    private async Task<long?> ResolveSteamIdAsync(string? login)
    {
        if (string.IsNullOrEmpty(login))
            return await Task.FromResult(0);

        var response = await _steamUserService.ResolveVanityUrlAsync(_steamContext.SteamApiKey, login);

        return response is { Response: { Success: 1 } } && long.TryParse(response.Response.SteamId, out var steamId) ? steamId : null;
    }
}