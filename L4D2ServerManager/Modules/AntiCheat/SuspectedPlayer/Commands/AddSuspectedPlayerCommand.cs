using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

public class AddSuspectedPlayerCommand
{
    public string? SuspectedPlayer { get; set; }
    public string? Login => this.Login();
    public string? SteamId => this.SteamId();
}