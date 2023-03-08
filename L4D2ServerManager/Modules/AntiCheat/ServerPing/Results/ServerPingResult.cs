namespace L4D2ServerManager.Modules.AntiCheat.ServerPing.Results;

public class ServerPingResult
{
    private readonly ServerPing? _serverPing;

    public ServerPingResult(ServerPing? serverPing)
    {
        _serverPing = serverPing;
    }

    public DateTime? When => _serverPing?.When;
    public bool IsOn => When.HasValue && When > DateTime.UtcNow.AddMinutes(-30);
    public bool IsOff => !IsOn;
}