using L4D2ServerManager.Users;
using L4D2ServerManager.Users.Constants;

namespace L4D2ServerManager.VirtualMachine.Extensions;

public static class VirtualMachineExtensions
{
    public static bool WasStartedBy(this IVirtualMachine virtualMachine, User user, int port)
    {
        return virtualMachine.StartedBy(port) == user.Id;
    }

    public static bool CanPowerOff(this IVirtualMachine virtualMachine)
    {
        return virtualMachine.Permissions.Contains(VirtualMachinePermissions.PowerOff);
    }
}