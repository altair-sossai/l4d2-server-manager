using Azure.Data.Tables;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Modules.Auth.Users.Commands;
using L4D2ServerManager.Modules.Auth.Users.Constants;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.ServerManager.Port.Services;
using L4D2ServerManager.Modules.ServerManager.Server;
using L4D2ServerManager.Modules.ServerManager.Server.Extensions;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace L4D2ServerManager.Modules.Auth.Users.Services;

public class UserService : IUserService
{
    private readonly IAzureTableStorageContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly IPortServer _portServer;
    private TableClient? _userTable;

    public UserService(IAzureTableStorageContext context,
        IMemoryCache memoryCache,
        IPortServer portServer)
    {
        _context = context;
        _memoryCache = memoryCache;
        _portServer = portServer;
    }

    private TableClient UserTable => _userTable ??= _context.GetTableClient("Users").Result;

    private List<User> Users => _memoryCache.GetOrCreate(nameof(Users), factory =>
    {
        factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

        return UserTable.Query<User>().ToList();
    });

    public User EnsureAuthentication(string token)
    {
        var command = new AuthenticationCommand(token);
        if (!command.Valid)
            throw new UnauthorizedAccessException();

        var user = Users.FirstOrDefault(user => user.RowKey == command.UserId && user.Secret == command.UserSecret);
        if (user == null)
            throw new UnauthorizedAccessException();

        return user;
    }

    public User EnsureAuthentication(string token, AccessLevel accessLevel)
    {
        var command = new AuthenticationCommand(token);
        if (!command.Valid)
            throw new UnauthorizedAccessException();

        var user = Users.FirstOrDefault(user => user.RowKey == command.UserId && user.Secret == command.UserSecret);
        if (user == null || !user.AccessLevel.HasFlag(accessLevel))
            throw new UnauthorizedAccessException();

        return user;
    }

    public void ApplyPermissions(User user, IVirtualMachine virtualMachine)
    {
        if (user.AccessLevel.HasFlag(AccessLevel.VirtualMachine))
        {
            ApplyAllPermissions(virtualMachine);
            return;
        }

        ApplyPowerOffPermission(user, virtualMachine);
    }

    public void ApplyPermissions(User user, IServer server)
    {
        if (user.AccessLevel.HasFlag(AccessLevel.VirtualMachine))
        {
            ApplyAllPermissions(server);
            return;
        }

        ApplyStopPermission(user, server);
        ApplyKickAllPlayersPermission(user, server);
        ApplyOpenPortPermission(user, server);
        ApplyClosePortPermission(user, server);
    }

    private static void ApplyAllPermissions(IVirtualMachine virtualMachine)
    {
        foreach (var permission in VirtualMachinePermissions.All)
            virtualMachine.Permissions.Add(permission);
    }

    private static void ApplyAllPermissions(IServer server)
    {
        foreach (var permission in ServerPermissions.All)
            server.Permissions.Add(permission);
    }

    private void ApplyPowerOffPermission(User user, IVirtualMachine virtualMachine)
    {
        var ports = _portServer.GetPorts(virtualMachine.IpAddress);
        var canPowerOff = ports.All(port => !port.IsRunning || virtualMachine.WasStartedBy(user, port.PortNumber));
        if (!canPowerOff)
            return;

        virtualMachine.Permissions.Add(VirtualMachinePermissions.PowerOff);
    }

    private static void ApplyStopPermission(User user, IServer server)
    {
        ApplyServerPermission(user, server, ServerPermissions.Stop);
    }

    private static void ApplyKickAllPlayersPermission(User user, IServer server)
    {
        ApplyServerPermission(user, server, ServerPermissions.KickAllPlayers);
    }

    private static void ApplyOpenPortPermission(User user, IServer server)
    {
        ApplyServerPermission(user, server, ServerPermissions.OpenPort);
    }

    private static void ApplyClosePortPermission(User user, IServer server)
    {
        ApplyServerPermission(user, server, ServerPermissions.ClosePort);
    }

    private static void ApplyServerPermission(User user, IServer server, string permission)
    {
        if (!server.WasStartedBy(user))
            return;

        server.Permissions.Add(permission);
    }
}