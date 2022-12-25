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
		var suspectedPlayerSecret = Find(communityId);

		return suspectedPlayerSecret != null;
	}

	public bool Validate(long communityId, string secret)
	{
		var suspectedPlayerSecret = Find(communityId);

		return suspectedPlayerSecret != null && suspectedPlayerSecret.Secret == secret;
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

	private SuspectedPlayerSecret? Find(long communityId)
	{
		return _memoryCache.GetOrCreate($"SuspectedPlayerSecret_{communityId}", factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

			return TableClient.Query<SuspectedPlayerSecret>(q => q.PartitionKey == PartitionKey && q.RowKey == communityId.ToString()).FirstOrDefault();
		});
	}
}