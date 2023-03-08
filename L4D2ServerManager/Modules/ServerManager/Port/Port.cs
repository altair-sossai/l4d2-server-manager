using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Modules.ServerManager.Port;

public class Port
{
    public Port(int portNumber, ServerInfo? serverInfo)
    {
        PortNumber = portNumber;
        ServerInfo = serverInfo;
    }

    public int PortNumber { get; }
    public ServerInfo? ServerInfo { get; }
    public bool IsRunning => ServerInfo != null;
    public int ConnectedPlayers => ServerInfo?.Players ?? 0;
}