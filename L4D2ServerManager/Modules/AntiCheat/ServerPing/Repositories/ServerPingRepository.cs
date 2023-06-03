using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace L4D2ServerManager.Modules.AntiCheat.ServerPing.Repositories;

public class ServerPingRepository : BaseTableStorageRepository<ServerPing>, IServerPingRepository
{
    private readonly IMemoryCache _memoryCache;

    public ServerPingRepository(IMemoryCache memoryCache,
        IAzureTableStorageContext tableContext)
        : base("ServerPing", tableContext)
    {
        _memoryCache = memoryCache;
    }

    public ServerPing? Get()
    {
        return _memoryCache.GetOrCreate("ServerPing", factory =>
        {
            factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            return Find("shared", "shared");
        });
    }

    public override void AddOrUpdate(ServerPing entity)
    {
        base.AddOrUpdate(entity);

        _memoryCache.Remove("ServerPing");
    }
}