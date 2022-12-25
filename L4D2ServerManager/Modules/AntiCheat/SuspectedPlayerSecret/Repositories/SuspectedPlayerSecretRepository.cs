using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerCache.Services;
using Microsoft.Extensions.Caching.Memory;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

public class SuspectedPlayerSecretRepository : BaseTableStorageRepository<SuspectedPlayerSecret>, ISuspectedPlayerSecretRepository
{
	private const string PartitionKey = "shared";
	private readonly IMemoryCache _memoryCache;
	private readonly ISuspectedPlayerCache _suspectedPlayerCache;

	public SuspectedPlayerSecretRepository(IMemoryCache memoryCache,
		IAzureTableStorageContext tableContext,
		ISuspectedPlayerCache suspectedPlayerCache)
		: base("SuspectedPlayerSecret", tableContext)
	{
		_memoryCache = memoryCache;
		_suspectedPlayerCache = suspectedPlayerCache;
	}

	public bool Exists(long communityId)
	{
		return _memoryCache.GetOrCreate($"SuspectedPlayerSecret_Exists_{communityId}", factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

			return Exists(PartitionKey, communityId.ToString());
		});
	}

	public bool Validate(long communityId, string secret)
	{
		return _memoryCache.GetOrCreate($"SuspectedPlayerSecret_Validate_{communityId}", factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

			return TableClient.Query<SuspectedPlayerSecret>(q => q.PartitionKey == PartitionKey && q.RowKey == communityId.ToString() && q.Secret == secret).Any();
		});
	}

	public override void Add(SuspectedPlayerSecret suspectedPlayerSecret)
	{
		base.Add(suspectedPlayerSecret);

		_suspectedPlayerCache.ClearAllKeys(suspectedPlayerSecret.CommunityId);
	}

	public void Delete(long communityId)
	{
		Delete(PartitionKey, communityId.ToString());

		_suspectedPlayerCache.ClearAllKeys(communityId);
	}
}