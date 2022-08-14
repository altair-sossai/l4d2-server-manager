using Azure;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using L4D2ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.VirtualMachine.Results;
using L4D2ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.VirtualMachine;

public class VirtualMachine : IVirtualMachine
{
    private readonly NetworkSecurityGroupResource _networkSecurityGroupResource;
    private readonly PublicIPAddressData _publicIpAddress;
    private readonly VirtualMachineResource _virtualMachine;

    public VirtualMachine(VirtualMachineResource virtualMachine,
        PublicIPAddressData publicIpAddress,
        NetworkSecurityGroupResource networkSecurityGroupResource)
    {
        _virtualMachine = virtualMachine;
        _publicIpAddress = publicIpAddress;
        _networkSecurityGroupResource = networkSecurityGroupResource;
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

    public async Task<PortInfo> GetPortInfoAsync(int port)
    {
        var securityRule = await _networkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        return new PortInfo(securityRuleData);
    }

    public async Task OpenPortAsync(int port, string ranges)
    {
        var securityRule = await _networkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        securityRuleData.Access = SecurityRuleAccess.Allow;
        securityRuleData.SourceAddressPrefix = ranges;

        await securityRule.Value.UpdateAsync(WaitUntil.Completed, securityRuleData);
    }

    public async Task ClosePortAsync(int port)
    {
        var securityRule = await _networkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        securityRuleData.Access = SecurityRuleAccess.Deny;
        securityRuleData.SourceAddressPrefix = "*";

        await securityRule.Value.UpdateAsync(WaitUntil.Completed, securityRuleData);
    }
}