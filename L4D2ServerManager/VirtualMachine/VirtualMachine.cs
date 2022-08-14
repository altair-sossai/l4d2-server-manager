using Azure;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using L4D2ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.VirtualMachine.Results;

namespace L4D2ServerManager.VirtualMachine;

public class VirtualMachine : IVirtualMachine
{
    private readonly PublicIPAddressData _publicIpAddress;
    private readonly VirtualMachineResource _virtualMachine;

    public VirtualMachine(VirtualMachineResource virtualMachine, PublicIPAddressData publicIpAddress)
    {
        _virtualMachine = virtualMachine;
        _publicIpAddress = publicIpAddress;
    }

    public VirtualMachineStatus Status
    {
        get
        {
            var instanceView = _virtualMachine.InstanceView().Value;
            var statuses = instanceView.Statuses;

            if (statuses.Any(status => status.Code == "PowerState/running"))
                return VirtualMachineStatus.On;

            if (statuses.Any(status => status.Code == "PowerState/deallocated"))
                return VirtualMachineStatus.Off;

            return VirtualMachineStatus.Unknown;
        }
    }

    public bool IsOn => Status == VirtualMachineStatus.On;
    public bool IsOff => Status == VirtualMachineStatus.Off;
    public string IpAddress => IsOn ? _publicIpAddress.IPAddress : null!;

    public async Task PowerOnAsync()
    {
        if (IsOn)
            return;

        await _virtualMachine.PowerOnAsync(WaitUntil.Completed);
    }

    public async Task PowerOffAsync()
    {
        if (IsOff)
            return;

        await _virtualMachine.PowerOffAsync(WaitUntil.Completed);
    }

    public async Task<RunScriptResult> RunCommandAsync(RunScriptCommand command)
    {
        if (!IsOn)
            return RunScriptResult.Empty;

        var runCommandInput = new RunCommandInput("RunShellScript");

        foreach (var line in command.Script)
            runCommandInput.Script.Add(line);

        var runCommandResult = await _virtualMachine.RunCommandAsync(WaitUntil.Completed, runCommandInput);

        return new RunScriptResult(runCommandResult.Value);
    }
}