using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerActivityFunction
{
	private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
	private readonly ISuspectedPlayerActivityService _suspectedPlayerActivityService;
	private readonly IUserService _userService;

	public SuspectedPlayerActivityFunction(IUserService userService,
		ISuspectedPlayerActivityService suspectedPlayerActivityService,
		ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
	{
		_userService = userService;
		_suspectedPlayerActivityService = suspectedPlayerActivityService;
		_suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
	}

	[FunctionName(nameof(SuspectedPlayerActivityFunction) + "_" + nameof(Find))]
	public IActionResult Find([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-activity/{communityId:long}")] HttpRequest httpRequest, long communityId)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

			var activity = _suspectedPlayerActivityRepository.Find(communityId);
			if (activity == null)
				return new NotFoundResult();

			return new OkObjectResult(activity);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(SuspectedPlayerActivityFunction) + "_" + nameof(CheckAntiCheatUsage))]
	public async Task<IActionResult> CheckAntiCheatUsage([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-activity/check-anti-cheat-usage")] HttpRequest httpRequest)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

			var command = await httpRequest.DeserializeBodyAsync<CheckAntiCheatUsageCommand>();
			var result = _suspectedPlayerActivityService.CheckAntiCheatUsage(command);

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}
}