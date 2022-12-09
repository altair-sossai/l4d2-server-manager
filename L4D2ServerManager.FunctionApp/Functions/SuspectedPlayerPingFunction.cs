using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerPingFunction
{
    private readonly ISuspectedPlayerPingRepository _suspectedPlayerPingRepository;
    private readonly ISuspectedPlayerPingService _suspectedPlayerPingService;
    private readonly ISuspectedPlayerService _suspectedPlayerService;
    private readonly IUserService _userService;

    public SuspectedPlayerPingFunction(IUserService userService,
        ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerPingService suspectedPlayerPingService,
        ISuspectedPlayerPingRepository suspectedPlayerPingRepository)
    {
        _userService = userService;
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerPingService = suspectedPlayerPingService;
        _suspectedPlayerPingRepository = suspectedPlayerPingRepository;
    }

    [FunctionName(nameof(SuspectedPlayerPingFunction) + "_" + nameof(Find))]
    public IActionResult Find([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-ping/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

            var ping = _suspectedPlayerPingRepository.Find(communityId);
            if (ping == null)
                return new NotFoundResult();

            return new OkObjectResult(ping);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerPingFunction) + "_" + nameof(Ping))]
    public async Task<IActionResult> Ping([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-ping")] HttpRequest httpRequest)
    {
        try
        {
            var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(httpRequest.AuthorizationToken());
            var command = await httpRequest.DeserializeBodyAsync<PingCommand>();

            _suspectedPlayerPingService.Ping(suspectedPlayer.CommunityId, command);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }
}