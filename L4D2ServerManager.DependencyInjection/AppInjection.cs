using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace L4D2ServerManager.DependencyInjection;

public static class AppInjection
{
    public static void AddApp(this IServiceCollection serviceCollection)
    {
        var assemblies = new[]
        {
            Assembly.Load("L4D2ServerManager")
        };

        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses()
            .AsImplementedInterfaces(type => assemblies.Contains(type.Assembly)));
    }
}