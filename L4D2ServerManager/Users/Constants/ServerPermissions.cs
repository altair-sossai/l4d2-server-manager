namespace L4D2ServerManager.Users.Constants;

public static class ServerPermissions
{
    public const string Stop = "stop";
    public const string KickAllPlayers = "kick-all-players";
    public const string GivePills = "give-pills";
    public const string FillSurvivorsHealth = "fill-survivors-health";
    public const string OpenPort = "open-port";
    public const string ClosePort = "close-port";

    public static readonly HashSet<string> All = new()
    {
        Stop, KickAllPlayers, GivePills, FillSurvivorsHealth, OpenPort, ClosePort
    };
}