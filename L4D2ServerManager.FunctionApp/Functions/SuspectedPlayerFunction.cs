using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerFunction
{
	private readonly IMapper _mapper;
	private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
	private readonly ISuspectedPlayerPingRepository _suspectedPlayerPingRepository;
	private readonly ISuspectedPlayerProcessRepository _suspectedPlayerProcessRepository;
	private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;
	private readonly ISuspectedPlayerScreenshotService _suspectedPlayerScreenshotService;
	private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;
	private readonly ISuspectedPlayerService _suspectedPlayerService;
	private readonly IUserService _userService;

	public SuspectedPlayerFunction(IMapper mapper,
		IUserService userService,
		ISuspectedPlayerService suspectedPlayerService,
		ISuspectedPlayerScreenshotService suspectedPlayerScreenshotService,
		ISuspectedPlayerRepository suspectedPlayerRepository,
		ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository,
		ISuspectedPlayerPingRepository suspectedPlayerPingRepository,
		ISuspectedPlayerProcessRepository suspectedPlayerProcessRepository,
		ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
	{
		_mapper = mapper;
		_userService = userService;
		_suspectedPlayerService = suspectedPlayerService;
		_suspectedPlayerScreenshotService = suspectedPlayerScreenshotService;
		_suspectedPlayerRepository = suspectedPlayerRepository;
		_suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;
		_suspectedPlayerPingRepository = suspectedPlayerPingRepository;
		_suspectedPlayerProcessRepository = suspectedPlayerProcessRepository;
		_suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
	}

	[FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(GetSuspectedPlayers))]
	public IActionResult GetSuspectedPlayers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players")] HttpRequest httpRequest)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

			var suspectedPlayers = _suspectedPlayerRepository.GetSuspectedPlayers();
			var result = _mapper.Map<List<PlayerResult>>(suspectedPlayers);

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(GetSuspectedPlayer))]
	public IActionResult GetSuspectedPlayer([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players/{communityId:long}")] HttpRequest httpRequest,
		long communityId)
	{
		try
		{
			var suspectedPlayer = _suspectedPlayerRepository.GetSuspectedPlayer(communityId);
			if (suspectedPlayer == null)
				return new NotFoundResult();

			var result = _mapper.Map<PlayerResult>(suspectedPlayer);

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(AddOrUpdate))]
	public async Task<IActionResult> AddOrUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players")] HttpRequest httpRequest)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

			var command = await httpRequest.DeserializeBodyAsync<SuspectedPlayerCommand>();
			var suspectedPlayer = _suspectedPlayerService.AddOrUpdate(command);
			var result = _mapper.Map<PlayerResult>(suspectedPlayer);

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(Delete))]
	public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players/delete")] HttpRequest httpRequest)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

			var command = await httpRequest.DeserializeBodyAsync<SuspectedPlayerCommand>();
			var suspectedPlayer = _suspectedPlayerService.Find(command.Account);
			if (suspectedPlayer == null)
				return new NotFoundResult();

			await DeleteAllSuspectedPlayerData(suspectedPlayer.CommunityId);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(DeleteUsingCommunityId))]
	public async Task<IActionResult> DeleteUsingCommunityId([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players/{communityId:long}")] HttpRequest httpRequest, long communityId)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

			await DeleteAllSuspectedPlayerData(communityId);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	private async Task DeleteAllSuspectedPlayerData(long communityId)
	{
		_suspectedPlayerRepository.Delete(communityId);
		_suspectedPlayerSecretRepository.Delete(communityId);
		_suspectedPlayerPingRepository.Delete(communityId);
		_suspectedPlayerProcessRepository.Delete(communityId);
		_suspectedPlayerActivityRepository.Delete(communityId);

		await _suspectedPlayerScreenshotService.DeleteAllScreenshotsAsync(communityId);
	}

	[FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(Sync))]
	public void Sync([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
	{
		_suspectedPlayerService.Sync();
	}
}