﻿namespace L4D2ServerManager.Modules.ServerManager.Port.Extensions;

public static class PortExtensions
{
    public static bool HasAnyPlayerConnected(this IEnumerable<Port> ports)
    {
        return ports.Any(port => port.IsRunning && port is { ConnectedPlayers: > 0, MaxPlayers: > 10 });
    }
}