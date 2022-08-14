using Azure.Core;
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

    public VirtualMachine GetByName(string name)
    {
        var virtualMachines = _context.SubscriptionResource.GetVirtualMachines();
        var virtualMachine = virtualMachines.First(f => f.Data.Name == name);
        var networkProfile = virtualMachine.Data.NetworkProfile;
        var networkInterfaces = networkProfile.NetworkInterfaces;
        var networkInterfaceReference = networkInterfaces.First();
        var networkInterface = _context.ArmClient.GetNetworkInterfaceResource(new ResourceIdentifier(networkInterfaceReference.Id!));
        var ipConfigurations = networkInterface.GetNetworkInterfaceIPConfigurations();
        var ipConfiguration = ipConfigurations.First().Data;
        var publicIpAddresses = _context.SubscriptionResource.GetPublicIPAddresses();
        var publicIpAddress = publicIpAddresses.FirstOrDefault(f => f.Data.Id! == ipConfiguration.PublicIPAddress.Id)!.Data;

        return new VirtualMachine(virtualMachine, publicIpAddress);
    }
}