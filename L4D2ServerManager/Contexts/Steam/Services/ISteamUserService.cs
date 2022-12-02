using L4D2ServerManager.Contexts.Steam.Responses;
using L4D2ServerManager.Contexts.Steam.ValueObjects;
using Refit;

namespace L4D2ServerManager.Contexts.Steam.Services;

public interface ISteamUserService
{
    [Get("/ISteamUser/ResolveVanityURL/v0001")]
    Task<ResponseData<ResolveVanityUrl>> ResolveVanityUrlAsync([AliasAs("key")] string key, [AliasAs("vanityurl")] string vanityUrl);

    [Get("/ISteamUser/GetPlayerSummaries/v0002")]
    Task<ResponseData<PlayersInfo>> GetPlayerSummariesAsync([AliasAs("key")] string key, [AliasAs("steamids")] string steamIds);

    public async Task<long?> ResolveSteamIdAsync(string key, string? login)
    {
        if (string.IsNullOrEmpty(login))
            return await Task.FromResult(0);

        var response = await ResolveVanityUrlAsync(key, login);

        return response is { Response: { Success: 1 } } && long.TryParse(response.Response.SteamId, out var steamId) ? steamId : null;
    }
}