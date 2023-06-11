using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Commands;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.FunctionApp.Requests;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using L4D2ServerManager.Modules.ServerManager.Players.Services;
using L4D2ServerManager.Modules.ServerManager.Server.Extensions;
using L4D2ServerManager.Modules.ServerManager.Server.Services;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Services;
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
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            var server = _serverService.GetByPort(virtualMachine, port);

            _userService.ApplyPermissions(user, server);

            return new OkObjectResult(server);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(InfoAsync))]
    public async Task<IActionResult> InfoAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{ip}:{port}/info")] HttpRequest httpRequest,
        string ip, int port)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);

            var serverInfo = await _serverService.GetServerInfoAsync(ip, port);
            if (serverInfo == null)
                return new NotFoundResult();

            return new OkObjectResult(serverInfo);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Players))]
    public IActionResult Players([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "server/{ip}:{port}/players")] HttpRequest httpRequest,
        string ip, int port)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);

            var players = _playerService.GetPlayers(ip, port);

            return new OkObjectResult(players);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(RunLocked))]
    public IActionResult RunLocked([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/run")] HttpRequest httpRequest,
        int port)
    {
        try
        {
            lock (Lock)
            {
                var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
                var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
                var server = _serverService.GetByPort(virtualMachine, port);
                var request = httpRequest.DeserializeBody<RunServerRequest>();

                server.RunAsync(user, request.Campaign).Wait();

                return new OkResult();
            }
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(ResetMatch))]
    public IActionResult ResetMatch([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/reset-match")] HttpRequest httpRequest,
        int port)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);

            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            var server = _serverService.GetByPort(virtualMachine, port);
            var request = httpRequest.DeserializeBody<ResetMatchRequest>();

            server.ResetMatch(request.MatchName);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(Stop))]
    public IActionResult Stop([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/stop")] HttpRequest httpRequest,
        int port)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            var server = _serverService.GetByPort(virtualMachine, port);

            _userService.ApplyPermissions(user, server);

            if (!server.CanStop())
                throw new UnauthorizedAccessException();

            server.Stop();

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(OpenPortAsync))]
    public async Task<IActionResult> OpenPortAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/open-port")] HttpRequest httpRequest,
        int port)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            var server = _serverService.GetByPort(virtualMachine, port);

            _userService.ApplyPermissions(user, server);

            if (!server.CanOpenPort())
                throw new UnauthorizedAccessException();

            await server.OpenPortAsync();

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(ServerFunction) + "_" + nameof(ClosePortAsync))]
    public async Task<IActionResult> ClosePortAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "server/{port}/close-port")] HttpRequest httpRequest,
        int port)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            var server = _serverService.GetByPort(virtualMachine, port);

            _userService.ApplyPermissions(user, server);

            if (!server.CanClosePort())
                throw new UnauthorizedAccessException();

            var command = await httpRequest.DeserializeBodyAsync<ClosePortCommand>();

            await server.ClosePortAsync(command.AllowedIps);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }
}