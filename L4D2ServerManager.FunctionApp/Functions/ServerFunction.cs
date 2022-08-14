using System.Linq;
using System.Threading.Tasks;
using L4D2ServerManager.Players.Services;
using L4D2ServerManager.Server.Services;
using L4D2ServerManager.VirtualMachine.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.FunctionApp.Functions;

public class ServerFunction
{
    private readonly IConfiguration _configuration;
    private readonly IPlayerService _playerService;
    private readonly IServerService _serverService;
    private readonly IVirtualMachineService _virtualMachineService;

    public ServerFunction(IConfiguration configuration,
        IVirtualMachineService virtualMachineService,
        IServerService serverService,
        IPlayerService playerService)
    {
        _configuration = configuration;
        _virtualMachineService = virtualMachineService;
        _serverService = serverService;
        _playerService = playerService;
    }

    private string VirtualMachineName => _configuration.GetValue<string>(nameof(VirtualMachineName));

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{port}")] HttpRequest httpRequest,
        int port)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        return new OkObjectResult(server);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(InfoAsync))]
    public async Task<IActionResult> InfoAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{port}/info")] HttpRequest httpRequest,
        int port)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);
        var serverInfo = await _serverService.GetServerInfoAsync(server);

        return new OkObjectResult(serverInfo);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Players))]
    public IActionResult Players([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{port}/players")] HttpRequest httpRequest,
        int port)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);
        var players = _playerService.GetPlayers(server);

        return new OkObjectResult(players.ToList());
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(RunAsync))]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/run")] HttpRequest httpRequest,
        int port)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        await server.RunAsync();

        return new OkObjectResult(server);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(StopAsync))]
    public async Task<IActionResult> StopAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/stop")] HttpRequest httpRequest,
        int port)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        await server.StopAsync();

        return new OkObjectResult(server);
    }
}