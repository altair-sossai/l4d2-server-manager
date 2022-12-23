using AutoMapper;
using FluentValidation;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Commands;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Services;

public class PlayerIpService : IPlayerIpService
{
	private readonly IMapper _mapper;
	private readonly IPlayerIpRepository _playerIpRepository;
	private readonly IValidator<PlayerIp> _validator;

	public PlayerIpService(IMapper mapper,
		IPlayerIpRepository playerIpRepository,
		IValidator<PlayerIp> validator)
	{
		_mapper = mapper;
		_playerIpRepository = playerIpRepository;
		_validator = validator;
	}

	public PlayerIp AddOrUpdate(PlayerIpCommand command)
	{
		var playerIp = _mapper.Map<PlayerIp>(command);

		_validator.ValidateAndThrowAsync(playerIp).Wait();
		_playerIpRepository.AddOrUpdate(playerIp);

		return playerIp;
	}
}