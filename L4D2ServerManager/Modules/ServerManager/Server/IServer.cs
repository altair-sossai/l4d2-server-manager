using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.Modules.ServerManager.Server;

public interface IServer
{
    string IpAddress { get; }
    int Port { get; }
    bool IsRunning { get; }
    PortInfo PortInfo { get; }
    HashSet<string> Permissions { get; }
    string? StartedBy { get; }
    DateTime? StartedAt { get; }
    Task RunAsync(User user, Campaign campaign);
    void ResetMatch(string matchName);
    void Stop();
    Task OpenPortAsync();
    Task ClosePortAsync(IEnumerable<string> allowedIps);
}