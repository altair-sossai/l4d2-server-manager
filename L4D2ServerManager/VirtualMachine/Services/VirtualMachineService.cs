using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Network;
using L4D2ServerManager.Azure;

namespace L4D2ServerManager.VirtualMachine.Services;

public class VirtualMachineService : IVirtualMachineService
{
    private readonly IAzureContext _context;

    public VirtualMachineService(IAzureContext context)
    {
        _context = context;
    }

    public IVirtualMachine GetByName(string name)
    {
        var virtualMachines = _context.SubscriptionResource.GetVirtualMachines();
        var virtualMachine = virtualMachines.First(f => f.Data.Name == name);
        var networkProfile = virtualMachine.Data.NetworkProfile;
        var networkInterfaces = networkProfile.NetworkInterfaces;
        var networkInterfaceReference = networkInterfaces.First();
        var networkInterfaceResources = _context.SubscriptionResource.GetNetworkInterfaces();
        var networkInterfaceResource = networkInterfaceResources.First(f => f.Data.Id == networkInterfaceReference.Id);
        var ipConfigurations = networkInterfaceResource.GetNetworkInterfaceIPConfigurations();
        var ipConfiguration = ipConfigurations.First().Data;
        var publicIpAddresses = _context.SubscriptionResource.GetPublicIPAddresses();
        var publicIpAddress = publicIpAddresses.FirstOrDefault(f => f.Data.Id! == ipConfiguration.PublicIPAddress.Id)!.Data;
        var networkSecurityGroupData = networkInterfaceResource.Data.NetworkSecurityGroup;
        var networkSecurityGroups = _context.SubscriptionResource.GetNetworkSecurityGroups();
        var networkSecurityGroupResource = networkSecurityGroups.First(f => f.Data.Id == networkSecurityGroupData.Id);

        return new VirtualMachine(virtualMachine, publicIpAddress, networkSecurityGroupResource);
    }
}