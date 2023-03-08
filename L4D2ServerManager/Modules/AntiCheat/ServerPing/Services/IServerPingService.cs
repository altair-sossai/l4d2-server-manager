using L4D2ServerManager.Modules.AntiCheat.ServerPing.Results;

namespace L4D2ServerManager.Modules.AntiCheat.ServerPing.Services;

public interface IServerPingService
{
    ServerPingResult Get();
    void Ping();
}