﻿using System.Reflection;
using FluentValidation;
using L4D2ServerManager.Contexts.Steam;
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

        serviceCollection.AddAutoMapper(assemblies);
        serviceCollection.AddValidatorsFromAssemblies(assemblies);

        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses()
            .AsImplementedInterfaces(type => assemblies.Contains(type.Assembly)));

        serviceCollection.AddScoped(serviceProvider => serviceProvider.GetRequiredService<ISteamContext>().PlayerService);
        serviceCollection.AddScoped(serviceProvider => serviceProvider.GetRequiredService<ISteamContext>().ServerInfoService);
        serviceCollection.AddScoped(serviceProvider => serviceProvider.GetRequiredService<ISteamContext>().SteamUserService);
    }
}