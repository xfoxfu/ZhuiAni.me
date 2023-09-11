// based on https://github.com/berhir/AspNetCore.SpaYarp
// Licensed under the MIT License

using AspNetCore.SpaYarp;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds required services and configuration to use the SPA proxy.
    /// The services get only added if a "spa.proxy.json" file exists.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="action"></param>
    public static void AddSpaYarp(this IServiceCollection services, Action<SpaDevelopmentServerOptions> action)
    {
        services.AddHttpForwarder();
        services.AddSingleton<SpaProxyLaunchManager>();
        services.Configure<SpaDevelopmentServerOptions>(action);
    }
}

