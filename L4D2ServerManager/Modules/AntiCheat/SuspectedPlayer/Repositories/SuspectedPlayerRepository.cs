using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerCache.Services;
using Microsoft.Extensions.Caching.Memory;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;

public class SuspectedPlayerRepository : BaseTableStorageRepository<SuspectedPlayer>, ISuspectedPlayerRepository
{
	private const string PartitionKey = "shared";
	private readonly IMemoryCache _memoryCache;
	private readonly ISuspectedPlayerCache _suspectedPlayerCache;

	public SuspectedPlayerRepository(IMemoryCache memoryCache,
		IAzureTableStorageContext tableContext,
		ISuspectedPlayerCache suspectedPlayerCache)
		: base("SuspectedPlayer", tableContext)
	{
		_memoryCache = memoryCache;
		_suspectedPlayerCache = suspectedPlayerCache;
	}

	public bool Exists(long communityId)
	{
		return _memoryCache.GetOrCreate($"SuspectedPlayer_Exists_{communityId}", factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

			return Exists(PartitionKey, communityId.ToString());
		});
	}

	public SuspectedPlayer? GetSuspectedPlayer(long communityId)
	{
		return _memoryCache.GetOrCreate($"SuspectedPlayer_GetSuspectedPlayer_{communityId}", factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

			return communityId == 0 ? null : Find(PartitionKey, communityId.ToString());
		});
	}

	public IEnumerable<SuspectedPlayer> GetSuspectedPlayers()
	{
		return _memoryCache.GetOrCreate("SuspectedPlayer_All", factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

			return GetAll().OrderBy(o => o.Name);
		});
	}

	public override void AddOrUpdate(SuspectedPlayer entity)
	{
		base.AddOrUpdate(entity);

		_suspectedPlayerCache.ClearAllKeys(entity.CommunityId);
	}

	public void Delete(long communityId)
	{
		Delete(PartitionKey, communityId.ToString());

		_suspectedPlayerCache.ClearAllKeys(communityId);
	}
}