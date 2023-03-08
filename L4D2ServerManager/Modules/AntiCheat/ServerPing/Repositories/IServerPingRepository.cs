namespace L4D2ServerManager.Modules.AntiCheat.ServerPing.Repositories;

public interface IServerPingRepository
{
    ServerPing? Get();
    void AddOrUpdate(ServerPing serverPing);
}