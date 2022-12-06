namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;

public interface ISuspectedPlayerPingRepository
{
    void AddOrUpdate(SuspectedPlayerPing suspectedPlayerPing);
}