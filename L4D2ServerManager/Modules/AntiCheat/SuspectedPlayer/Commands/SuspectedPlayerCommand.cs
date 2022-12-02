using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

public class SuspectedPlayerCommand
{
    public string? SuspectedPlayer { get; set; }
    public string? Login => this.Login();
    public long? SteamId => this.SteamId();
}