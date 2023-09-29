using System.Reflection;
using L4D2ServerManager.Contexts.Steam.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace L4D2ServerManager;

public static class AppInjection
{
    public static void AddApp(this IServiceCollection serviceCollection)
    {
        var assemblies = new[]
        {
            Assembly.Load("L4D2ServerManager")
        };

        serviceCollection.AddMemoryCache();

        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses()
            .AsImplementedInterfaces(type => assemblies.Contains(type.Assembly)));

        serviceCollection.AddSteamContext();
    }
}