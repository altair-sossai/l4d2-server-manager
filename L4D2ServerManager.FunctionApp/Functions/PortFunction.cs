using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Port.Services;
using L4D2ServerManager.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class PortFunction
{
    private readonly IPortServer _portServer;
    private readonly IUserService _userService;

    public PortFunction(IUserService userService,
        IPortServer portServer)
    {
        _userService = userService;
        _portServer = portServer;
    }

    [FunctionName(nameof(PortFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ports/{ip}")] HttpRequest httpRequest,
        string ip)
    {
        _userService.EnsureAuthentication(httpRequest.AuthorizationToken());

        var ports = _portServer.GetPorts(ip);

        return new OkObjectResult(ports);
    }
}