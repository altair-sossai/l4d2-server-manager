using L4D2ServerManager.Users;

namespace L4D2ServerManager.VirtualMachine.Extensions;

public static class VirtualMachineExtensions
{
    public static bool WasStartedBy(this IVirtualMachine virtualMachine, User user, int port)
    {
        return virtualMachine.StartedBy(port) == user.Id;
    }
}