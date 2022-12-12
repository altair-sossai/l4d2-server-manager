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

namespace L4D2ServerManager.Modules.Auth.Users.Services;

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

	public User EnsureAuthentication(string token, AccessLevel accessLevel)
	{
		var command = new AuthenticationCommand(token);
		if (!command.Valid)
			throw new UnauthorizedAccessException();

		var user = UserTable.Query<User>(user => user.Id == command.UserId && user.Secret == command.UserSecret).FirstOrDefault();
		if (user == null || !user.AccessLevel.HasFlag(accessLevel))
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