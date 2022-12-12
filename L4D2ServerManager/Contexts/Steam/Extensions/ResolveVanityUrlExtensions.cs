using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Contexts.Steam.Extensions;

public static class ResolveVanityUrlExtensions
{
	public static long? SteamId(this ResolveVanityUrl? resolveVanityUrl)
	{
		if (resolveVanityUrl is not { Success: 1 })
			return null;

		return long.TryParse(resolveVanityUrl.SteamId, out var steamId) ? steamId : null;
	}
}