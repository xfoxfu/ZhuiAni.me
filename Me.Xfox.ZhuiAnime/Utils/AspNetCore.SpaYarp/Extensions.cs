// Based on https://github.com/berhir/AspNetCore.SpaYarp
//
// MIT License
// 
// Copyright (c) Bernd Hirschmann
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AspNetCore.SpaYarp;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds the middleware for the SPA proxy to the application's request pipeline
    /// and adds a "catch-all" route endpoint that forwards all requests to the SPA dev server.
    /// The middleware and route endpoint get only added if the 'spa.proxy.json' file exists and the SpaYarp services were added (there is a check for the SpaProxyLaunchManager).
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> used to configure the HTTP pipeline, and routes.</param>
    /// <returns>The <see cref="WebApplication"/>.</returns>
    public static WebApplication UseSpaYarp(this WebApplication app)
    {
        app.UseSpaYarpMiddleware();
        return app;
    }
}

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
        services.Configure(action);
        services.AddHostedService<SpaProxyLaunchTask>();
    }
}

public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the middleware for the SPA proxy to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance used to configure the request pipeline.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseSpaYarpMiddleware(this IApplicationBuilder app)
    {
        var spaProxyLaunchManager = app.ApplicationServices.GetService<SpaProxyLaunchManager>();

        if (spaProxyLaunchManager == null)
        {
            return app;
        }

        app.UseMiddleware<SpaProxyMiddleware>();

        return app;
    }
}
