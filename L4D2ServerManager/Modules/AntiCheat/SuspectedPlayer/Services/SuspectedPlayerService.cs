using AutoMapper;
using FluentValidation;
using L4D2ServerManager.Contexts.Steam.Helpers;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Contexts.Steam.Structures;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public class SuspectedPlayerService : ISuspectedPlayerService
{
	private readonly IMapper _mapper;
	private readonly ISteamIdService _steamIdService;
	private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;
	private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;
	private readonly IValidator<SuspectedPlayer> _validator;

	public SuspectedPlayerService(IMapper mapper,
		ISuspectedPlayerRepository suspectedPlayerRepository,
		ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository,
		IValidator<SuspectedPlayer> validator,
		ISteamIdService steamIdService)
	{
		_mapper = mapper;
		_suspectedPlayerRepository = suspectedPlayerRepository;
		_suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;
		_steamIdService = steamIdService;
		_validator = validator;
	}

	public SuspectedPlayer? Find(string? account)
	{
		if (string.IsNullOrEmpty(account))
			return null;

		SteamIdentifiers.TryParse(account, out var steamIdentifiers);

		var customUrl = SteamIdHelper.CustomUrl(account);
		var steamId = _steamIdService.ResolveSteamIdAsync(customUrl).Result ?? steamIdentifiers.CommunityId ?? 0;
		var suspectedPlayer = _suspectedPlayerRepository.GetSuspectedPlayer(steamId);

		return suspectedPlayer;
	}

	public SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command)
	{
		var suspectedPlayer = Find(command.Account) ?? SuspectedPlayer.Default;

		_mapper.Map(command, suspectedPlayer);
		_validator.ValidateAndThrowAsync(suspectedPlayer).Wait();
		_suspectedPlayerRepository.AddOrUpdate(suspectedPlayer);

		return suspectedPlayer;
	}

	public SuspectedPlayer EnsureAuthentication(string accessToken)
	{
		var command = new SuspectedPlayerAuthenticationCommand(accessToken);
		if (!command.Valid)
			throw new UnauthorizedAccessException();

		var valid = _suspectedPlayerSecretRepository.Validate(command.CommunityId, command.Secret!);
		if (!valid)
			throw new UnauthorizedAccessException();

		var suspectedPlayer = _suspectedPlayerRepository.GetSuspectedPlayer(command.CommunityId);
		if (suspectedPlayer == null)
			throw new UnauthorizedAccessException();

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