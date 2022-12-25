using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Modules.Steam.Services;

public interface ISteamService
{
	Task<GamesInfo?> GetOwnedGamesAsync(long communityId);
}