namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Commands;

public class CheckAntiCheatUsageCommand
{
    public HashSet<string> Suspecteds { get; set; } = new();
}