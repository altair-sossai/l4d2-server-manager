using L4D2ServerManager.Contexts.Steam.Extensions;

namespace L4D2ServerManager.Contexts.Steam.Services;

public class SteamIdService : ISteamIdService
{
	private readonly ISteamContext _steamContext;
	private readonly ISteamUserService _steamUserService;

	public SteamIdService(ISteamContext steamContext, ISteamUserService steamUserService)
	{
		_steamContext = steamContext;
		_steamUserService = steamUserService;
	}

	public async Task<long?> ResolveSteamIdAsync(string? customUrl)
	{
		if (string.IsNullOrEmpty(customUrl))
			return await Task.FromResult((long?)null);

		var responseData = await _steamUserService.ResolveVanityUrlAsync(_steamContext.SteamApiKey, customUrl);

		return responseData.Response?.SteamId();
	}
}