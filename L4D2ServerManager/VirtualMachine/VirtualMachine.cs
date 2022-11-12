﻿using Azure;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using L4D2ServerManager.Contexts.AzureSubscription;
using L4D2ServerManager.Users;
using L4D2ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.VirtualMachine.Extensions;
using L4D2ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.VirtualMachine;

public class VirtualMachine : IVirtualMachine
{
    private static readonly object Lock = new();
    private readonly IAzureSubscriptionContext _context;
    private readonly string _virtualMachineName;
    private NetworkInterfaceResource? _networkInterfaceResource;
    private NetworkSecurityGroupResource? _networkSecurityGroupResource;
    private PublicIPAddressData? _publicIpAddress;
    private VirtualMachineResource? _virtualMachineResource;

    public VirtualMachine(IAzureSubscriptionContext context, string virtualMachineName)
    {
        _context = context;
        _virtualMachineName = virtualMachineName;
    }

    private NetworkSecurityGroupResource NetworkSecurityGroupResource
    {
        get
        {
            if (_networkSecurityGroupResource != null)
                return _networkSecurityGroupResource;

            var networkSecurityGroupData = NetworkInterfaceResource.Data.NetworkSecurityGroup;
            var networkSecurityGroups = _context.SubscriptionResource.GetNetworkSecurityGroups();
            var networkSecurityGroupResource = networkSecurityGroups.First(f => f.Data.Id == networkSecurityGroupData.Id);

            return _networkSecurityGroupResource = networkSecurityGroupResource;
        }
    }

    private PublicIPAddressData PublicIpAddress
    {
        get
        {
            if (_publicIpAddress != null)
                return _publicIpAddress;


            var ipConfigurations = NetworkInterfaceResource.GetNetworkInterfaceIPConfigurations();
            var ipConfiguration = ipConfigurations.First().Data;
            var publicIpAddresses = _context.SubscriptionResource.GetPublicIPAddresses();
            var publicIpAddress = publicIpAddresses.FirstOrDefault(f => f.Data.Id! == ipConfiguration.PublicIPAddress.Id)!.Data;

            return _publicIpAddress = publicIpAddress;
        }
    }

    private NetworkInterfaceResource NetworkInterfaceResource
    {
        get
        {
            if (_networkInterfaceResource != null)
                return _networkInterfaceResource;

            var networkProfile = VirtualMachineResource.Data.NetworkProfile;
            var networkInterfaces = networkProfile.NetworkInterfaces;
            var networkInterfaceReference = networkInterfaces.First();
            var networkInterfaceResources = _context.SubscriptionResource.GetNetworkInterfaces();
            var networkInterfaceResource = networkInterfaceResources.First(f => f.Data.Id == networkInterfaceReference.Id);

            return _networkInterfaceResource = networkInterfaceResource;
        }
    }

    private VirtualMachineResource VirtualMachineResource
    {
        get
        {
            if (_virtualMachineResource != null)
                return _virtualMachineResource;

            var virtualMachines = _context.SubscriptionResource.GetVirtualMachines();
            var virtualMachine = virtualMachines.First(f => f.Data.Name == _virtualMachineName);

            return _virtualMachineResource = virtualMachine;
        }
    }

    public VirtualMachineStatus Status
    {
        get
        {
            for (var seconds = 5; seconds < 15; seconds++)
            {
                var instanceView = VirtualMachineResource.InstanceView().Value;
                var statuses = instanceView.Statuses;

                if (statuses.Any(status => status.Code == "PowerState/running"))
                    return VirtualMachineStatus.On;

                if (statuses.Any(status => status.Code == "PowerState/deallocated"))
                    return VirtualMachineStatus.Off;

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            return VirtualMachineStatus.Unknown;
        }
    }

    public bool IsOn => Status == VirtualMachineStatus.On;
    public bool IsOff => Status == VirtualMachineStatus.Off;
    public string IpAddress => IsOn ? PublicIpAddress.IPAddress : null!;
    public HashSet<string> Permissions { get; } = new();

    public string? PowerOnBy
    {
        get
        {
            const string key = "power-on-by";

            var tags = VirtualMachineResource.Data.Tags;

            return tags.ContainsKey(key) ? tags[key] : null;
        }
    }

    public DateTime? PowerOnAt
    {
        get
        {
            const string key = "power-on-at";

            var tags = VirtualMachineResource.Data.Tags;

            if (!tags.ContainsKey(key))
                return null;

            return DateTime.TryParse(tags[key], out var date) ? date : null;
        }
    }

    public async Task PowerOnAsync(User user)
    {
        if (IsOn)
            return;

        await VirtualMachineResource.PowerOnAsync(WaitUntil.Completed);

        var values = new Dictionary<string, string>
        {
            { "power-on-by", user.Id },
            { "power-on-at", DateTime.UtcNow.ToString("O") }
        };

        await UpdateTagsAsync(values);
    }

    public async Task PowerOffAsync()
    {
        if (IsOff)
            return;

        await VirtualMachineResource.DeallocateAsync(WaitUntil.Completed);
    }

    public void RunCommand(RunScriptCommand command)
    {
        if (!IsOn)
            return;

        var runCommandInput = new RunCommandInput("RunShellScript");

        foreach (var line in command.Script)
            runCommandInput.Script.Add(line);

        RunCommandLocked(runCommandInput);
    }

    public async Task<PortInfo> GetPortInfoAsync(int port)
    {
        var securityRule = await NetworkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        return new PortInfo(securityRuleData);
    }

    public async Task OpenPortAsync(int port, string ranges)
    {
        var securityRule = await NetworkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        securityRuleData.Access = SecurityRuleAccess.Allow;
        securityRuleData.SourceAddressPrefix = ranges;

        await securityRule.Value.UpdateAsync(WaitUntil.Completed, securityRuleData);
    }

    public async Task ClosePortAsync(int port)
    {
        var securityRule = await NetworkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        securityRuleData.Access = SecurityRuleAccess.Deny;
        securityRuleData.SourceAddressPrefix = "*";

        await securityRule.Value.UpdateAsync(WaitUntil.Completed, securityRuleData);
    }

    public async Task UpdateTagsAsync(IDictionary<string, string> values)
    {
        await VirtualMachineResource.UpdateTagsAsync(values);
    }

    public string? StartedBy(int port)
    {
        var key = $"port-{port}-started-by";
        var tags = VirtualMachineResource.Data.Tags;

        return tags.ContainsKey(key) ? tags[key] : null;
    }

    public DateTime? StartedAt(int port)
    {
        var key = $"port-{port}-started-at";
        var tags = VirtualMachineResource.Data.Tags;

        if (!tags.ContainsKey(key))
            return null;

        return DateTime.TryParse(tags[key], out var date) ? date : null;
    }

    private void RunCommandLocked(RunCommandInput runCommandInput)
    {
        lock (Lock)
        {
            VirtualMachineResource.RunCommandAsync(WaitUntil.Completed, runCommandInput).Wait();
        }
    }
}