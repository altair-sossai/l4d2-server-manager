using System;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerFileCheckSuccessFunction
{
	private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
	private readonly ISuspectedPlayerService _suspectedPlayerService;

	public SuspectedPlayerFileCheckSuccessFunction(ISuspectedPlayerService suspectedPlayerService,
		ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
	{
		_suspectedPlayerService = suspectedPlayerService;
		_suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
	}

	[FunctionName(nameof(SuspectedPlayerFileCheckSuccessFunction) + "_" + nameof(Post))]
	public IActionResult Post([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-file-check-success")] HttpRequest httpRequest)
	{
		try
		{
			var accessToken = httpRequest.AuthorizationToken();
			var appId = httpRequest.AppId();
			var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(accessToken, appId);
			_suspectedPlayerActivityRepository.FileCheckSuccess(suspectedPlayer.CommunityId);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}
}