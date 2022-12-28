using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;
using L4D2ServerManager.Modules.AntiCheat.Player.Services;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Commands;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Repositories;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Results;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class PlayerIpFunction
{
	private readonly IMapper _mapper;
	private readonly IPlayerIpRepository _playerIpRepository;
	private readonly IPlayerIpService _playerIpService;
	private readonly IPlayerService _playerService;
	private readonly IUserService _userService;

	public PlayerIpFunction(IMapper mapper,
		IUserService userService,
		IPlayerService playerService,
		IPlayerIpService playerIpService,
		IPlayerIpRepository playerIpRepository)
	{
		_mapper = mapper;
		_userService = userService;
		_playerService = playerService;
		_playerIpService = playerIpService;
		_playerIpRepository = playerIpRepository;
	}

	[FunctionName(nameof(PlayerIpFunction) + "_" + nameof(GetAllPlayerIps))]
	public IActionResult GetAllPlayerIps([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "player-ip/{communityId:long}/ips")] HttpRequest httpRequest, long communityId)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

			var playerIps = _playerIpRepository.GetAllPlayerIps(communityId);
			var result = _mapper.Map<List<IpResult>>(playerIps);

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(PlayerIpFunction) + "_" + nameof(GetAllPlayersWithIp))]
	public IActionResult GetAllPlayersWithIp([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "player-ip/{ip}/players")] HttpRequest httpRequest, string ip)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

			var playerIps = _playerIpRepository.GetAllPlayersWithIp(ip);
			var result = _mapper.Map<List<PlayerResult>>(playerIps);

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(PlayerIpFunction) + "_" + nameof(AddOrUpdate))]
	public async Task<IActionResult> AddOrUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "player-ip")] HttpRequest httpRequest)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

			var command = await httpRequest.DeserializeBodyAsync<PlayerIpCommand>();
			_playerIpService.AddOrUpdate(command);

			var player = _playerService.Find(command.CommunityId);
			var playerIps = _playerIpRepository.GetAllPlayersWithIp(command.Ip!, command.CommunityId);
			var withSameIp = _mapper.Map<List<PlayerResult>>(playerIps);

			return new OkObjectResult(new { player, ip = command.Ip, withSameIp });
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(PlayerIpFunction) + "_" + nameof(Delete))]
	public IActionResult Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "player-ip/{communityId:long}")] HttpRequest httpRequest, long communityId)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);
			_playerIpRepository.Delete(communityId);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(PlayerIpFunction) + "_" + nameof(DeleteOldIps))]
	public void DeleteOldIps([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
	{
		_playerIpRepository.DeleteOldIps();
	}
}