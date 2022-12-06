namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

public interface ISuspectedPlayerSecretRepository
{
    bool Exists(long communityId);
    bool Validate(long communityId, string secret);
    void Add(SuspectedPlayerSecret suspectedPlayerSecret);
    void Delete(long communityId);
}