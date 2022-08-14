using System.Threading.Tasks;
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
    private readonly IVirtualMachineService _virtualMachineService;

    public VirtualMachineFunction(IConfiguration configuration,
        IVirtualMachineService virtualMachineService)
    {
        _configuration = configuration;
        _virtualMachineService = virtualMachineService;
    }

    private string VirtualMachineName => _configuration.GetValue<string>(nameof(VirtualMachineName));

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "virtual-machine")] HttpRequest httpRequest)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

        return new OkObjectResult(virtualMachine);
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(PowerOnAsync))]
    public async Task<IActionResult> PowerOnAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/power-on")] HttpRequest httpRequest)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

        await virtualMachine.PowerOnAsync();

        return new OkObjectResult(virtualMachine);
    }

    [FunctionName(nameof(VirtualMachineFunction) + "_" + nameof(PowerOffAsync))]
    public async Task<IActionResult> PowerOffAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "virtual-machine/power-off")] HttpRequest httpRequest)
    {
        var virtualMachine = _virtualMachineService.GetByName(VirtualMachineName);

        await virtualMachine.PowerOffAsync();

        return new OkObjectResult(virtualMachine);
    }
}