using Microsoft.Extensions.DependencyInjection;

namespace L4D2ServerManager.Contexts.Steam.DependencyInjection;

public static class SteamContextDependencyInjection
{
    public static void AddSteamContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(serviceProvider => serviceProvider.GetRequiredService<ISteamContext>().PlayerService);
        serviceCollection.AddScoped(serviceProvider => serviceProvider.GetRequiredService<ISteamContext>().ServerInfoService);
        serviceCollection.AddScoped(serviceProvider => serviceProvider.GetRequiredService<ISteamContext>().SteamUserService);
    }
}