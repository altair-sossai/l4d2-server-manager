using L4D2ServerManager.Modules.AntiCheat.ServerPing.Repositories;
using L4D2ServerManager.Modules.AntiCheat.ServerPing.Results;

namespace L4D2ServerManager.Modules.AntiCheat.ServerPing.Services;

public class ServerPingService : IServerPingService
{
    private readonly IServerPingRepository _serverPingRepository;

    public ServerPingService(IServerPingRepository serverPingRepository)
    {
        _serverPingRepository = serverPingRepository;
    }

    public ServerPingResult Get()
    {
        var serverPing = _serverPingRepository.Get();

        return new ServerPingResult(serverPing);
    }

    public void Ping()
    {
        var ping = new ServerPing();

        _serverPingRepository.AddOrUpdate(ping);
    }
}