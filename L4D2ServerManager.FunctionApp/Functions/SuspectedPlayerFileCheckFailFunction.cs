using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerFileCheckFailFunction
{
	private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
	private readonly ISuspectedPlayerFileFailRepository _suspectedPlayerFileFailRepository;
	private readonly ISuspectedPlayerFileFailService _suspectedPlayerFileFailService;
	private readonly ISuspectedPlayerService _suspectedPlayerService;
	private readonly IUserService _userService;

	public SuspectedPlayerFileCheckFailFunction(IUserService userService,
		ISuspectedPlayerService suspectedPlayerService,
		ISuspectedPlayerFileFailService suspectedPlayerFileFailService,
		ISuspectedPlayerFileFailRepository suspectedPlayerFileFailRepository,
		ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
	{
		_userService = userService;
		_suspectedPlayerService = suspectedPlayerService;
		_suspectedPlayerFileFailService = suspectedPlayerFileFailService;
		_suspectedPlayerFileFailRepository = suspectedPlayerFileFailRepository;
		_suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
	}

	[FunctionName(nameof(SuspectedPlayerFileCheckFailFunction) + "_" + nameof(PostAsync))]
	public async Task<IActionResult> PostAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-file-check-fail")] HttpRequest httpRequest)
	{
		try
		{
			var accessToken = httpRequest.AuthorizationToken();
			var appId = httpRequest.AppId();
			var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(accessToken, appId);
			_suspectedPlayerActivityRepository.FileCheckFail(suspectedPlayer.CommunityId);

			var commands = await httpRequest.DeserializeBodyAsync<List<SuspectedPlayerFileFailCommand>>();
			_suspectedPlayerFileFailService.BatchOperation(suspectedPlayer.CommunityId, commands);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFileCheckFailFunction) + "_" + nameof(GetAllFiles))]
	public IActionResult GetAllFiles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-file-check-fail/{communityId:long}")] HttpRequest httpRequest, long communityId)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

			var files = _suspectedPlayerFileFailRepository.GetAllFiles(communityId);

			return new OkObjectResult(files.OrderBy(o => o.File).ToList());
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFileCheckFailFunction) + "_" + nameof(Delete))]
	public IActionResult Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players-file-check-fail/{communityId:long}")] HttpRequest httpRequest, long communityId)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);
			_suspectedPlayerFileFailRepository.Delete(communityId);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerFileCheckFailFunction) + "_" + nameof(DeleteOldFiles))]
	public void DeleteOldFiles([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
	{
		_suspectedPlayerFileFailRepository.DeleteOldFiles();
	}
}