namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;

public interface ISuspectedPlayerRepository
{
    bool Exists(long communityId);
    SuspectedPlayer? GetSuspectedPlayer(long communityId);
    IEnumerable<SuspectedPlayer> GetSuspectedPlayers();
    void AddOrUpdate(SuspectedPlayer suspectedPlayer);
    void Delete(long communityId);
}