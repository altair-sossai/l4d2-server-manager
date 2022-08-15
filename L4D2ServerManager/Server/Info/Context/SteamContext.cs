using System.Text.Json;
using L4D2ServerManager.Server.Info.Services;
using Refit;

namespace L4D2ServerManager.Server.Info.Context;

public static class SteamContext
{
    private const string BaseUrl = "https://api.steampowered.com";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private static readonly RefitSettings Settings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(Options)
    };

    public static IServerInfoService ServerInfoService => CreateService<IServerInfoService>();

    private static T CreateService<T>()
    {
        return RestService.For<T>(BaseUrl, Settings);
    }
}