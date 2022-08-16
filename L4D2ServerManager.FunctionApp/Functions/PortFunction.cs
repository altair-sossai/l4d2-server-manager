using L4D2ServerManager.FunctionApp.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.FunctionApp.Functions;

public class PortFunction
{
    private readonly IConfiguration _configuration;

    public PortFunction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string AuthorizationKey => _configuration.GetValue<string>(nameof(AuthorizationKey));

    [FunctionName(nameof(PortFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ports")] HttpRequest httpRequest)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var ports = new[] {27016, 27017, 27018};

        return new OkObjectResult(ports);
    }
}