using System.Linq;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Port.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.FunctionApp.Functions;

public class PortFunction
{
    private readonly IConfiguration _configuration;
    private readonly IPortServer _portServer;

    public PortFunction(IConfiguration configuration,
        IPortServer portServer)
    {
        _configuration = configuration;
        _portServer = portServer;
    }

    private string AuthorizationKey => _configuration.GetValue<string>(nameof(AuthorizationKey));

    [FunctionName(nameof(PortFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ports/{ip}")] HttpRequest httpRequest,
        string ip)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var ports = _portServer.GetPorts(ip).ToList();

        return new OkObjectResult(ports);
    }
}