using System;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.ServerPing.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class ServerPingFunction
{
	private readonly IServerPingService _serverPingService;
	private readonly ISuspectedPlayerService _suspectedPlayerService;
	private readonly IUserService _userService;

	public ServerPingFunction(IUserService userService,
		ISuspectedPlayerService suspectedPlayerService,
		IServerPingService serverPingService)
	{
		_userService = userService;
		_suspectedPlayerService = suspectedPlayerService;
		_serverPingService = serverPingService;
	}

	[FunctionName(nameof(ServerPingFunction) + "_" + nameof(Get))]
	public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server-ping")] HttpRequest httpRequest)
	{
		try
		{
			_suspectedPlayerService.EnsureAuthentication(httpRequest.AuthorizationToken());

			var result = _serverPingService.Get();

			return new OkObjectResult(result);
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}

	[FunctionName(nameof(ServerPingFunction) + "_" + nameof(Ping))]
	public IActionResult Ping([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "server-ping")] HttpRequest httpRequest)
	{
		try
		{
			_userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);
			_serverPingService.Ping();

			return new OkResult();
		}
		catch (Exception exception)
		{
			return ErrorResult.Build(exception).ResponseMessageResult();
		}
	}
}