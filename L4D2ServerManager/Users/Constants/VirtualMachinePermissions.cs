namespace L4D2ServerManager.Users.Constants;

public static class VirtualMachinePermissions
{
    public const string PowerOff = "power-off";

    public static readonly HashSet<string> All = new()
    {
        PowerOff
    };
}