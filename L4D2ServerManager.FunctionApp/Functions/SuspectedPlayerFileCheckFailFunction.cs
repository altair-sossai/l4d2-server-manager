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

public class SuspectedPlayerFileCheckFailFunction
{
	private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
	private readonly ISuspectedPlayerService _suspectedPlayerService;

	public SuspectedPlayerFileCheckFailFunction(ISuspectedPlayerService suspectedPlayerService,
		ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
	{
		_suspectedPlayerService = suspectedPlayerService;
		_suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
	}

	[FunctionName(nameof(SuspectedPlayerFileCheckFailFunction) + "_" + nameof(Post))]
	public IActionResult Post([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-file-check-fail")] HttpRequest httpRequest)
	{
		try
		{
			var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(httpRequest.AuthorizationToken());
			_suspectedPlayerActivityRepository.FileCheckFail(suspectedPlayer.CommunityId);

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}
}