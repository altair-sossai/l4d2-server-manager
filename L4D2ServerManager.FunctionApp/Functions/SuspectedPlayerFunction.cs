using System;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerFunction
{
    private readonly ISuspectedPlayerService _suspectedPlayerService;
    private readonly IUserService _userService;

    public SuspectedPlayerFunction(IUserService userService,
        ISuspectedPlayerService suspectedPlayerService)
    {
        _userService = userService;
        _suspectedPlayerService = suspectedPlayerService;
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players")] HttpRequest httpRequest)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

            var suspectedPlayers = _suspectedPlayerService.GetSuspectedPlayers();

            return new OkObjectResult(suspectedPlayers);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(GetById))]
    public IActionResult GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players/{steamId}")] HttpRequest httpRequest,
        string steamId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

            var suspectedPlayer = _suspectedPlayerService.GetSuspectedPlayer(steamId);
            if (suspectedPlayer == null)
                return new NotFoundResult();

            return new OkObjectResult(suspectedPlayer);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }
}