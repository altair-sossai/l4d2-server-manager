using AutoMapper;
using Azure.Data.Tables;
using FluentValidation;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public class SuspectedPlayerService : ISuspectedPlayerService
{
    private static bool _created;
    private readonly IMapper _mapper;
    private readonly IAzureTableStorageContext _tableContext;
    private readonly IValidator<SuspectedPlayer> _validator;
    private TableClient? _userTable;

    public SuspectedPlayerService(IMapper mapper,
        IAzureTableStorageContext tableContext,
        IValidator<SuspectedPlayer> validator)
    {
        _mapper = mapper;
        _tableContext = tableContext;
        _validator = validator;

        CreateIfNotExists();
    }

    private TableClient SuspectedPlayerTable => _userTable ??= _tableContext.GetTableClient("SuspectedPlayers").Result;

    public SuspectedPlayer? GetSuspectedPlayer(string? steamId)
    {
        return string.IsNullOrEmpty(steamId) ? null : SuspectedPlayerTable.Query<SuspectedPlayer>(q => q.RowKey == steamId).FirstOrDefault();
    }

    public IEnumerable<SuspectedPlayer> GetSuspectedPlayers()
    {
        return SuspectedPlayerTable.Query<SuspectedPlayer>();
    }

    public SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command)
    {
        var suspectedPlayer = GetSuspectedPlayer(command.SteamId) ?? SuspectedPlayer.Default;

        _mapper.Map(command, suspectedPlayer);
        _validator.ValidateAndThrowAsync(suspectedPlayer).Wait();

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