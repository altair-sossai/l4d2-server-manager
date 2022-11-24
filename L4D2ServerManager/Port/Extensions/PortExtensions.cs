namespace L4D2ServerManager.Port.Extensions;

public static class PortExtensions
{
    public static bool HasAnyPlayerConnected(this IEnumerable<Port> ports)
    {
        return ports.Any(port => port.ConnectedPlayers > 0);
    }
}