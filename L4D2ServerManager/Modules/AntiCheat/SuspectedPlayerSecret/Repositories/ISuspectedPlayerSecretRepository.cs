namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

public interface ISuspectedPlayerSecretRepository
{
    bool Exists(long communityId);
    void Add(SuspectedPlayerSecret suspectedPlayerSecret);
    void Delete(long communityId);
}