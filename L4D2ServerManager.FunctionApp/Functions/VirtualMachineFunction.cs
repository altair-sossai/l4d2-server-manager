using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using L4D2ServerManager.Modules.ServerManager.Port.Extensions;
using L4D2ServerManager.Modules.ServerManager.Port.Services;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Extensions;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.FunctionApp.Functions;

public class VirtualMachineFunction
{
    private readonly IConfiguration _configuration;
    private readonly IPortServer _portServer;
    private readonly IUserService _userService;
    private readonly IVirtualMachineService _virtualMachineService;

    public VirtualMachineFunction(IConfiguration configuration,
        IUserService userService,
        IVirtualMachineService virtualMachineService,
        IPortServer portServer)
    {
        _configuration = configuration;
        _userService = userService;
        _virtualMachineService = virtualMachineService;
        _portServer = portServer;
    }

    private string VirtualMachineName => _configuration.GetValue<string>(nameof(VirtualMachineName));
    private int AttemptsToShutdown => _configuration.GetValue(nameof(AttemptsToShutdown), 3);

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "virtual-machine")] HttpRequest httpRequest)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

            _userService.ApplyPermissions(user, virtualMachine);

            return new OkObjectResult(virtualMachine);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(Info))]
    public IActionResult Info([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "virtual-machine/info")] HttpRequest httpRequest)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);

            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

            return new OkObjectResult(virtualMachine);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(PowerOnAsync))]
    public async Task<IActionResult> PowerOnAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/power-on")] HttpRequest httpRequest)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            await virtualMachine.PowerOnAsync(user);

            _userService.ApplyPermissions(user, virtualMachine);

            return new OkObjectResult(virtualMachine);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(PowerOffAsync))]
    public async Task<IActionResult> PowerOffAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/power-off")] HttpRequest httpRequest)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);
            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

            _userService.ApplyPermissions(user, virtualMachine);

            if (!virtualMachine.CanPowerOff())
                throw new UnauthorizedAccessException();

            await virtualMachine.PowerOffAsync(user);

            return new OkObjectResult(virtualMachine);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(TryPowerOffAsync))]
    public async Task<IActionResult> TryPowerOffAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/try-power-off")] HttpRequest httpRequest)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.Servers);

            var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
            if (virtualMachine.IsOff)
            {
                await virtualMachine.ClearShutdownAttemptAsync();
                return new OkObjectResult(virtualMachine);
            }

            var ports = _portServer.GetPorts(virtualMachine.IpAddress);
            if (ports.HasAnyPlayerConnected())
            {
                await virtualMachine.ClearShutdownAttemptAsync();
                return new OkObjectResult(virtualMachine);
            }

            if (virtualMachine.ShutdownAttempt >= AttemptsToShutdown)
            {
                await virtualMachine.PowerOffAsync(user);
                return new OkObjectResult(virtualMachine);
            }

            await virtualMachine.IncrementShutdownAttemptAsync();

            return new OkObjectResult(virtualMachine);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }
}