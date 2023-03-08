using Microsoft.Extensions.Caching.Memory;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerCache.Services;

public class SuspectedPlayerCache : ISuspectedPlayerCache
{
    private readonly IMemoryCache _memoryCache;

    public SuspectedPlayerCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void ClearAllKeys(long communityId)
    {
        _memoryCache.Remove("SuspectedPlayer_All");
        _memoryCache.Remove($"SuspectedPlayer_Exists_{communityId}");
        _memoryCache.Remove($"SuspectedPlayer_GetSuspectedPlayer_{communityId}");
        _memoryCache.Remove($"SuspectedPlayerSecret_{communityId}");
    }
}