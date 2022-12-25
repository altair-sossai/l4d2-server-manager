using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.Services;
using L4D2ServerManager.Contexts.Steam.ValueObjects;
using Microsoft.Extensions.Caching.Memory;

namespace L4D2ServerManager.Modules.Steam.Services;

public class SteamService : ISteamService
{
	private readonly IMemoryCache _memoryCache;
	private readonly ISteamContext _steamContext;
	private readonly ISteamPlayerService _steamPlayerService;

	public SteamService(IMemoryCache memoryCache,
		ISteamContext steamContext,
		ISteamPlayerService steamPlayerService)
	{
		_memoryCache = memoryCache;
		_steamContext = steamContext;
		_steamPlayerService = steamPlayerService;
	}

	public Task<GamesInfo?> GetOwnedGamesAsync(long communityId)
	{
		return _memoryCache.GetOrCreateAsync($"GetOwnedGames_{communityId}", async factory =>
		{
			factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

			var responseData = await _steamPlayerService.GetOwnedGamesAsync(_steamContext.SteamApiKey, communityId.ToString());

			return responseData.Response;
		});
	}
}