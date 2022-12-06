using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerPingFunction
{
    private readonly ISuspectedPlayerPingService _suspectedPlayerPingService;
    private readonly ISuspectedPlayerService _suspectedPlayerService;

    public SuspectedPlayerPingFunction(ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerPingService suspectedPlayerPingService)
    {
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerPingService = suspectedPlayerPingService;
    }

    [FunctionName(nameof(SuspectedPlayerPingFunction) + "_" + nameof(Ping))]
    public async Task<IActionResult> Ping([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-Ping")] HttpRequest httpRequest)
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