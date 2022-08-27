using Azure.Data.Tables;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Port.Services;
using L4D2ServerManager.Users.Commands;
using L4D2ServerManager.Users.Constants;
using L4D2ServerManager.VirtualMachine;
using L4D2ServerManager.VirtualMachine.Extensions;

namespace L4D2ServerManager.Users.Services;

public class UserService : IUserService
{
    private readonly IAzureTableStorageContext _context;
    private readonly IPortServer _portServer;
    private TableClient? _userTable;

    public UserService(IAzureTableStorageContext context,
        IPortServer portServer)
    {
        _context = context;
        _portServer = portServer;
    }

    private TableClient UserTable => _userTable ??= _context.GetTableClient("Users").Result;

    public User EnsureAuthentication(string token)
    {
        var command = new AuthenticationCommand(token);
        if (!command.Valid)
            throw new UnauthorizedAccessException();

        var user = UserTable.Query<User>(user => user.Id == command.UserId && user.Secret == command.UserSecret).FirstOrDefault();
        if (user == null)
            throw new UnauthorizedAccessException();

        return user;
    }

    public User? GetUser(string userId)
    {
        return UserTable.Query<User>(user => user.Id == userId).FirstOrDefault();
    }

    public IEnumerable<User> GetUsers()
    {
        return UserTable.Query<User>();
    }

    public void ApplyPermissions(User user, IVirtualMachine virtualMachine)
    {
        if (user.Admin)
        {
            ApplyAllPermissions(virtualMachine);
            return;
        }

        ApplyPowerOffPermission(user, virtualMachine);
    }

    private static void ApplyAllPermissions(IVirtualMachine virtualMachine)
    {
        virtualMachine.Permissions.Add(VirtualMachinePermissions.PowerOff);
    }

    private void ApplyPowerOffPermission(User user, IVirtualMachine virtualMachine)
    {
        var ports = _portServer.GetPorts(virtualMachine.IpAddress);
        var canPowerOff = ports.All(port => !port.IsRunning || virtualMachine.WasStartedBy(user, port.PortNumber));
        if (!canPowerOff)
            return;

        virtualMachine.Permissions.Add(VirtualMachinePermissions.PowerOff);
    }
}