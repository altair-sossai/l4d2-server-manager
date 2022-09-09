using L4D2ServerManager.Users;
using L4D2ServerManager.Users.Constants;

namespace L4D2ServerManager.Server.Extensions;

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

    public static bool CanKickAllPlayers(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.KickAllPlayers);
    }

    public static bool CanGivePills(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.GivePills);
    }

    public static bool CanOpenPort(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.OpenPort);
    }

    public static bool CanClosePort(this IServer server)
    {
        return server.Permissions.Contains(ServerPermissions.ClosePort);
    }
}