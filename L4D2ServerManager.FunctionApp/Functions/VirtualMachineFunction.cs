using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Users.Services;
using L4D2ServerManager.VirtualMachine.Extensions;
using L4D2ServerManager.VirtualMachine.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.FunctionApp.Functions;

public class VirtualMachineFunction
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IVirtualMachineService _virtualMachineService;

    public VirtualMachineFunction(IConfiguration configuration,
        IUserService userService,
        IVirtualMachineService virtualMachineService)
    {
        _configuration = configuration;
        _userService = userService;
        _virtualMachineService = virtualMachineService;
    }

    private string VirtualMachineName => _configuration.GetValue<string>(nameof(VirtualMachineName));

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "virtual-machine")] HttpRequest httpRequest)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

        _userService.ApplyPermissions(user, virtualMachine);

        return new OkObjectResult(virtualMachine);
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(PowerOnAsync))]
    public async Task<IActionResult> PowerOnAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/power-on")] HttpRequest httpRequest)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);
        await virtualMachine.PowerOnAsync(user);

        _userService.ApplyPermissions(user, virtualMachine);

        return new OkObjectResult(virtualMachine);
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(PowerOffAsync))]
    public async Task<IActionResult> PowerOffAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/power-off")] HttpRequest httpRequest)
    {
        var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

        _userService.ApplyPermissions(user, virtualMachine);

        if (!virtualMachine.CanPowerOff())
            throw new UnauthorizedAccessException();

        await virtualMachine.PowerOffAsync();

        return new OkObjectResult(virtualMachine);
    }
}