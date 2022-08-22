using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Commands;
using L4D2ServerManager.FunctionApp.Extensions;
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
    private string AuthorizationKey => _configuration.GetValue<string>(nameof(AuthorizationKey));

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{port}")] HttpRequest httpRequest,
        int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        return new OkObjectResult(server);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(InfoAsync))]
    public async Task<IActionResult> InfoAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{ip}:{port}/info")] HttpRequest httpRequest,
        string ip, int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var serverInfo = await _serverService.GetServerInfoAsync(ip, port);
        if (serverInfo == null)
            return new NotFoundResult();

        return new OkObjectResult(serverInfo);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Players))]
    public IActionResult Players([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{ip}:{port}/players")] HttpRequest httpRequest,
        string ip, int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var players = _playerService.GetPlayers(ip, port);

        return new OkObjectResult(players);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Run))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/run")] HttpRequest httpRequest,
        int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        server.Run();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Stop))]
    public IActionResult Stop([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/stop")] HttpRequest httpRequest,
        int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        server.Stop();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(KickAllPlayers))]
    public IActionResult KickAllPlayers([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/kick-all-players")] HttpRequest httpRequest,
        int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        server.KickAllPlayers();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(OpenPortAsync))]
    public async Task<IActionResult> OpenPortAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/open-port")] HttpRequest httpRequest,
        int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var command = await httpRequest.DeserializeBodyAsync<OpenPortCommand>();
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        await server.OpenPortAsync(command.Ranges);

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(ClosePortAsync))]
    public async Task<IActionResult> ClosePortAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/close-port")] HttpRequest httpRequest,
        int port)
    {
        httpRequest.EnsureAuthentication(AuthorizationKey);

        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        await server.ClosePortAsync();

        return new OkResult();
    }
}