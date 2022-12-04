using AutoMapper;
using Azure.Data.Tables;
using FluentValidation;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public class SuspectedPlayerService : ISuspectedPlayerService
{
    private static bool _created;
    private readonly IMapper _mapper;
    private readonly ISteamIdService _steamIdService;
    private readonly IAzureTableStorageContext _tableContext;
    private readonly IValidator<SuspectedPlayer> _validator;
    private TableClient? _suspectedPlayerTable;

    public SuspectedPlayerService(IMapper mapper,
        ISteamIdService steamIdService,
        IAzureTableStorageContext tableContext,
        IValidator<SuspectedPlayer> validator)
    {
        _mapper = mapper;
        _steamIdService = steamIdService;
        _tableContext = tableContext;
        _validator = validator;

        CreateIfNotExists();
    }

    private TableClient SuspectedPlayerTable => _suspectedPlayerTable ??= _tableContext.GetTableClient("SuspectedPlayer").Result;

    public SuspectedPlayer? GetSuspectedPlayer(long communityId)
    {
        return communityId == 0 ? null : SuspectedPlayerTable.Query<SuspectedPlayer>(q => q.RowKey == communityId.ToString()).FirstOrDefault();
    }

    public IEnumerable<SuspectedPlayer> GetSuspectedPlayers()
    {
        return SuspectedPlayerTable.Query<SuspectedPlayer>().OrderBy(o => o.Name);
    }

    public SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command)
    {
        var steamId = _steamIdService.ResolveSteamIdAsync(command.CustomUrl).Result ?? command.CommunityId ?? 0;
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
                Account = suspectedPlayer.CommunityId.ToString()
            };

            AddOrUpdate(suspectedPlayerCommand);
        }
    }

    public void Delete(long communityId)
    {
        SuspectedPlayerTable.DeleteEntity("shared", communityId.ToString());
    }

    private void CreateIfNotExists()
    {
        if (_created)
            return;

        SuspectedPlayerTable.CreateIfNotExists();

        _created = true;
    }
}