﻿using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Enums;

namespace L4D2ServerManager.Modules.ServerManager.Server;

public interface IServer
{
    string IpAddress { get; }
    int Port { get; }
    bool IsRunning { get; }
    PortStatus PortStatus { get; }
    HashSet<string> Permissions { get; }
    string? StartedBy { get; }
    DateTime? StartedAt { get; }
    Task RunAsync(User user, Campaign campaign);
    Task StopAsync();
    Task OpenPortAsync();
    Task ClosePortAsync(IEnumerable<string> allowedIps);
    Task OpenSlotAsync();
}