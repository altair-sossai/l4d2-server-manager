using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.Auth.Users.Constants;

namespace L4D2ServerManager.Modules.ServerManager.Server.Extensions;

public static class ServerExtensions
{
    public static bool WasStartedBy(this IServer server, User user)
    {
        return server.StartedBy == user.Id;
    }

    public static bool CanStop(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.Stop);
    }

    public static bool CanOpenPort(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.OpenPort);
    }

    public static bool CanClosePort(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.ClosePort);
    }

    public static bool CanOpenSlot(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.OpenSlot);
    }
}