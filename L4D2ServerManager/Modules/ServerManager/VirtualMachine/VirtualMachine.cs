﻿using Azure;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using L4D2ServerManager.Contexts.AzureSubscription;
using L4D2ServerManager.Infrastructure.Helpers;
using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Extensions;
using Polly;
using Polly.Retry;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine;

public class VirtualMachine : IVirtualMachine
{
    private static readonly AsyncRetryPolicy RetryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    private readonly IAzureSubscriptionContext _context;
    private readonly string _virtualMachineName;
    private NetworkInterfaceResource? _networkInterfaceResource;
    private NetworkSecurityGroupResource? _networkSecurityGroupResource;
    private PublicIPAddressData? _publicIpAddress;
    private VirtualMachineInstanceView? _virtualMachineInstanceView;
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

    private VirtualMachineInstanceView VirtualMachineInstanceView
    {
        get
        {
            if (_virtualMachineInstanceView != null)
                return _virtualMachineInstanceView;

            return _virtualMachineInstanceView = VirtualMachineResource.InstanceView().Value;
        }
    }

    public VirtualMachineStatus Status
    {
        get
        {
            for (var attempt = 0; attempt <= 15; attempt++)
            {
                var instanceView = VirtualMachineInstanceView;
                var statuses = instanceView.Statuses;

                if (statuses.Any(status => status.Code == "PowerState/running"))
                    return VirtualMachineStatus.On;

                if (statuses.Any(status => status.Code == "PowerState/deallocated"))
                    return VirtualMachineStatus.Off;

                Thread.Sleep(TimeSpan.FromSeconds(1));
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

            return tags.TryGetValue(key, out var tag) ? tag : null;
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

    public string? PowerOffBy
    {
        get
        {
            const string key = "power-off-by";

            var tags = VirtualMachineResource.Data.Tags;

            return tags.TryGetValue(key, out var tag) ? tag : null;
        }
    }

    public DateTime? PowerOffAt
    {
        get
        {
            const string key = "power-off-at";

            var tags = VirtualMachineResource.Data.Tags;

            if (!tags.ContainsKey(key))
                return null;

            return DateTime.TryParse(tags[key], out var date) ? date : null;
        }
    }

    public int ShutdownAttempt
    {
        get
        {
            const string key = "shutdown-attempt";

            var tags = VirtualMachineResource.Data.Tags;

            if (!tags.ContainsKey(key))
                return 0;

            return int.TryParse(tags[key], out var shutdownAttempt) ? shutdownAttempt : 0;
        }
    }

    public async Task RestartAsync(User user)
    {
        await PowerOffAsync(user);
        await PowerOnAsync(user);
    }

    public async Task PowerOnAsync(User user)
    {
        if (IsOn)
            return;

        await VirtualMachineResource.PowerOnAsync(WaitUntil.Completed);

        _virtualMachineInstanceView = null;

        var values = new Dictionary<string, string>
        {
            { "power-on-by", user.Id },
            { "power-on-at", DateTime.UtcNow.ToString("u") },
            { "shutdown-attempt", "0" }
        };

        await UpdateTagsAsync(values);
    }

    public async Task PowerOffAsync(User user)
    {
        if (IsOff)
            return;

        _virtualMachineInstanceView = null;

        await VirtualMachineResource.DeallocateAsync(WaitUntil.Completed);

        var values = new Dictionary<string, string>
        {
            { "power-off-by", user.Id },
            { "power-off-at", DateTime.UtcNow.ToString("u") },
            { "shutdown-attempt", "0" }
        };

        await UpdateTagsAsync(values);
    }

    public async Task RunCommandAsync(RunScriptCommand command)
    {
        if (!IsOn)
            return;

        var input = new RunCommandInput("RunShellScript");

        foreach (var line in command.Script)
            input.Script.Add(line);

        await RunCommandWithRetryAsync(input);
    }

    public async Task<PortStatus> GetPortStatusAsync(int port)
    {
        var securityRule = await NetworkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        return securityRuleData.Access == SecurityRuleAccess.Allow && securityRuleData.SourceAddressPrefix == "*" ? PortStatus.Open : PortStatus.Close;
    }

    public async Task OpenPortAsync(int port)
    {
        var securityRule = await NetworkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        securityRuleData.Access = SecurityRuleAccess.Allow;
        securityRuleData.SourceAddressPrefix = "*";
        securityRuleData.SourceAddressPrefixes.Clear();

        await securityRule.Value.UpdateAsync(WaitUntil.Completed, securityRuleData);

        await UpdateIpTablesRulesAsync();
    }

    public async Task ClosePortAsync(int port, IEnumerable<string> allowedIps)
    {
        var ips = allowedIps.Where(IpHelper.IsValidIpv4).ToList();
        if (ips.Count == 0)
            throw new Exception("Provide at least one IP");

        var securityRule = await NetworkSecurityGroupResource.GetSecurityRuleAsync(port.ToString());
        var securityRuleData = securityRule.Value.Data;

        securityRuleData.Access = SecurityRuleAccess.Allow;
        securityRuleData.SourceAddressPrefix = null;
        securityRuleData.SourceAddressPrefixes.Clear();

        if (ips.Count == 1)
            securityRuleData.SourceAddressPrefix = ips.First();
        else
        {
            foreach (var ip in ips)
                securityRuleData.SourceAddressPrefixes.Add(ip);
        }

        await securityRule.Value.UpdateAsync(WaitUntil.Completed, securityRuleData);

        await UpdateIpTablesRulesAsync();
    }

    public async Task UpdateTagsAsync(IDictionary<string, string> values)
    {
        await VirtualMachineResource.UpdateTagsAsync(values);

        ClearVirtualMachineCache();
    }

    public string? StartedBy(int port)
    {
        var key = $"port-{port}-started-by";
        var tags = VirtualMachineResource.Data.Tags;

        return tags.TryGetValue(key, out var tag) ? tag : null;
    }

    public DateTime? StartedAt(int port)
    {
        var key = $"port-{port}-started-at";
        var tags = VirtualMachineResource.Data.Tags;

        if (!tags.ContainsKey(key))
            return null;

        return DateTime.TryParse(tags[key], out var date) ? date : null;
    }

    public async Task ClearShutdownAttemptAsync()
    {
        if (ShutdownAttempt == 0)
            return;

        var values = new Dictionary<string, string>
        {
            { "shutdown-attempt", "0" }
        };

        await UpdateTagsAsync(values);
    }

    public async Task IncrementShutdownAttemptAsync()
    {
        var values = new Dictionary<string, string>
        {
            { "shutdown-attempt", $"{ShutdownAttempt + 1}" }
        };

        await UpdateTagsAsync(values);
    }

    private async Task RunCommandWithRetryAsync(RunCommandInput command)
    {
        await RetryPolicy.ExecuteAsync(async () => { await VirtualMachineResource.RunCommandAsync(WaitUntil.Completed, command); });
    }

    private void ClearVirtualMachineCache()
    {
        _networkInterfaceResource = null;
        _networkSecurityGroupResource = null;
        _publicIpAddress = null;
        _virtualMachineInstanceView = null;
        _virtualMachineResource = null;
    }

    private async Task UpdateIpTablesRulesAsync()
    {
        ClearVirtualMachineCache();

        var securityRules = NetworkSecurityGroupResource.GetSecurityRules();
        var command = new IpTablesRulesCommand(securityRules);

        await RunCommandAsync(command);
    }
}