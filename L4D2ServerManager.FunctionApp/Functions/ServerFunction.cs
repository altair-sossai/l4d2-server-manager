using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Commands;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Players.Services;
using L4D2ServerManager.Server.Extensions;
using L4D2ServerManager.Server.Services;
using L4D2ServerManager.Users.Services;
using L4D2ServerManager.VirtualMachine.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.FunctionApp.Functions;

public class ServerFunction
{
    private static readonly object Lock = new();
    private readonly IConfiguration _configuration;
    private readonly IPlayerService _playerService;
    private readonly IServerService _serverService;
    private readonly IUserService _userService;
    private readonly IVirtualMachineService _virtualMachineService;

    public ServerFunction(IConfiguration configuration,
        IUserService userService,
        IVirtualMachineService virtualMachineService,
        IServerService serverService,
        IPlayerService playerService)
    {
        _configuration = configuration;
        _userService = userService;
        _virtualMachineService = virtualMachineService;
        _serverService = serverService;
        _playerService = playerService;
    }

    private string VirtualMachineName => _configuration.GetValue<string>(nameof(VirtualMachineName));

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{port}")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        return new OkObjectResult(server);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(InfoAsync))]
    public async Task<IActionResult> InfoAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{ip}:{port}/info")] HttpRequest httpRequest,
        string ip, int port)
    {
        _userService.EnsureAuthentication(httpRequest.AuthorizationToken());

        var serverInfo = await _serverService.GetServerInfoAsync(ip, port);
        if (serverInfo == null)
            return new NotFoundResult();

        return new OkObjectResult(serverInfo);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Players))]
    public IActionResult Players([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{ip}:{port}/players")] HttpRequest httpRequest,
        string ip, int port)
    {
        _userService.EnsureAuthentication(httpRequest.AuthorizationToken());

        var players = _playerService.GetPlayers(ip, port);

        return new OkObjectResult(players);
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(RunLocked))]
    public IActionResult RunLocked([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/run")] HttpRequest httpRequest,
        int port)
    {
        lock (Lock)
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            var server = _serverService.GetByPort(virtualMachine, port);

            server.RunAsync(user).Wait();

            return new OkResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Stop))]
    public IActionResult Stop([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/stop")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        if (!server.CanStop())
            throw new UnauthorizedAccessException();

        server.Stop();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(KickAllPlayers))]
    public IActionResult KickAllPlayers([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/kick-all-players")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        if (!server.CanKickAllPlayers())
            throw new UnauthorizedAccessException();

        server.KickAllPlayers();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(GivePills))]
    public IActionResult GivePills([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/give-pills")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        if (!server.CanGivePills())
            throw new UnauthorizedAccessException();

        server.GivePills();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(FillSurvivorsHealth))]
    public IActionResult FillSurvivorsHealth([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/fill-survivors-health")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        if (!server.CanFillSurvivorsHealth())
            throw new UnauthorizedAccessException();

        server.FillSurvivorsHealth();

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(OpenPortAsync))]
    public async Task<IActionResult> OpenPortAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/open-port")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var command = await httpRequest.DeserializeBodyAsync<OpenPortCommand>();
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        if (!server.CanOpenPort())
            throw new UnauthorizedAccessException();

        await server.OpenPortAsync(command.Ranges);

        return new OkResult();
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(ClosePortAsync))]
    public async Task<IActionResult> ClosePortAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/close-port")] HttpRequest httpRequest,
        int port)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        var server = _serverService.GetByPort(virtualMachine, port);

        _userService.ApplyPermissions(user, server);

        if (!server.CanClosePort())
            throw new UnauthorizedAccessException();

        await server.ClosePortAsync();

        return new OkResult();
    }
}