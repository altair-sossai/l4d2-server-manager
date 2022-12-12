using L4D2ServerManager.Contexts.Steam.Responses;
using L4D2ServerManager.Contexts.Steam.ValueObjects;
using Refit;

namespace L4D2ServerManager.Contexts.Steam.Services;

public interface IPlayerService
{
	[Get("/IPlayerService/GetOwnedGames/v0001")]
	Task<ResponseData<GamesInfo>> GetOwnedGamesAsync([AliasAs("key")] string key, [AliasAs("steamid")] string steamId);
}