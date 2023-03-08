namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerCache.Services;

public interface ISuspectedPlayerCache
{
    void ClearAllKeys(long communityId);
}