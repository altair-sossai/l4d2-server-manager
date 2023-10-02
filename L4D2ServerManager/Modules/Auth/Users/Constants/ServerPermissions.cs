namespace L4D2ServerManager.Modules.Auth.Users.Constants;

public static class ServerPermissions
{
    public const string Stop = "stop";
    public const string OpenPort = "open-port";
    public const string ClosePort = "close-port";
    public const string OpenSlot = "open-slot";

    public static readonly HashSet<string> All = new()
    {
        Stop, OpenPort, ClosePort, OpenSlot
    };
}