using AutoMapper;
using FluentValidation;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public class SuspectedPlayerService : ISuspectedPlayerService
{
    private readonly IMapper _mapper;
    private readonly ISteamIdService _steamIdService;
    private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;
    private readonly IValidator<SuspectedPlayer> _validator;

    public SuspectedPlayerService(IMapper mapper,
        ISuspectedPlayerRepository suspectedPlayerRepository,
        IValidator<SuspectedPlayer> validator,
        ISteamIdService steamIdService)
    {
        _mapper = mapper;
        _suspectedPlayerRepository = suspectedPlayerRepository;
        _steamIdService = steamIdService;
        _validator = validator;
    }

    public SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command)
    {
        var steamId = _steamIdService.ResolveSteamIdAsync(command.CustomUrl).Result ?? command.CommunityId ?? 0;
        var suspectedPlayer = _suspectedPlayerRepository.GetSuspectedPlayer(steamId) ?? SuspectedPlayer.Default;

        _mapper.Map(command, suspectedPlayer);
        _validator.ValidateAndThrowAsync(suspectedPlayer).Wait();
        _suspectedPlayerRepository.AddOrUpdate(suspectedPlayer);

        return suspectedPlayer;
    }

    public void Sync()
    {
        foreach (var suspectedPlayer in _suspectedPlayerRepository.GetSuspectedPlayers())
        {
            var suspectedPlayerCommand = new SuspectedPlayerCommand
            {
                Account = suspectedPlayer.CommunityId.ToString()
            };

            AddOrUpdate(suspectedPlayerCommand);
        }
    }
}