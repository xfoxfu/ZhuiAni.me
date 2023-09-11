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
//
// based on https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/Spa/SpaProxy/src/SpaProxyMiddleware.cs

using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text;
using Yarp.ReverseProxy.Forwarder;

namespace AspNetCore.SpaYarp;
/// <summary>
/// Middleware to display a page while the SPA proxy is launching and redirect to the proxy url once the proxy is
/// ready or we have given up trying to start it.
/// This is to help Visual Studio work well in several scenarios by allowing VS to:
/// 1) Launch on the URL configured for the backend (we handle the redirect to the proxy when ready).
/// 2) Ensure that the server is up and running quickly instead of waiting for the proxy to be ready to start the
///    server which causes Visual Studio to think the app failed to launch.
/// </summary>
public class SpaProxyMiddleware
{
    private static bool _spaClientRunning = false;

    private readonly RequestDelegate _next;
    private readonly SpaProxyLaunchManager _spaProxyLaunchManager;
    private readonly IOptions<SpaDevelopmentServerOptions> _options;
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly ILogger<SpaProxyMiddleware> _logger;
    private readonly HttpMessageInvoker _httpClient;

    private readonly SuppressOriginalHostTransformer _transformer;
    private readonly ForwarderRequestConfig _requestOptions;


    public SpaProxyMiddleware(
        RequestDelegate next,
        SpaProxyLaunchManager spaProxyLaunchManager,
        IOptions<SpaDevelopmentServerOptions> options,
        IHostApplicationLifetime hostLifetime,
        ILogger<SpaProxyMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _spaProxyLaunchManager = spaProxyLaunchManager ?? throw new ArgumentNullException(nameof(spaProxyLaunchManager));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
        });
        _transformer = new SuppressOriginalHostTransformer();
        _requestOptions = new ForwarderRequestConfig
        {
            ActivityTimeout = TimeSpan.FromSeconds(100)
        };
    }

    protected bool IsFallbackEndpoint(Endpoint? endpoint)
    {
        if (endpoint == null) return true;
        if (endpoint is RouteEndpoint route)
        {
            if (route.RoutePattern.RawText == "{*path:nonfile}") return true;
        }
        return false;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!IsFallbackEndpoint(context.GetEndpoint())) { await _next(context); return; }
        if (!_spaClientRunning && !await _spaProxyLaunchManager.IsSpaClientRunning(context.RequestAborted))
        {
            _spaProxyLaunchManager.StartInBackground(_hostLifetime.ApplicationStopping);
            _logger.LogDebug("SPA client is not ready. Returning temporary landing page.");
            context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate, max-age=0";
            context.Response.ContentType = "text/html";

            await using var writer = new StreamWriter(context.Response.Body, Encoding.UTF8);
            await writer.WriteAsync(GenerateSpaLaunchPage(_options.Value));
        }
        else
        {
            _logger.LogDebug($"SPA client is ready.");
            _spaClientRunning = true;

            // configure the proxy
            var forwarder = context.RequestServices.GetRequiredService<IHttpForwarder>();

            // Configure our own HttpMessageInvoker for outbound calls for proxy operations
            var error = await forwarder.SendAsync(context, _options.Value.ClientUrl, _httpClient, _requestOptions, _transformer);
            // Check if the proxy operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = context.Features.Get<IForwarderErrorFeature>();
                var exception = errorFeature?.Exception;
            }
        }

    }

    protected string GenerateSpaLaunchPage(SpaDevelopmentServerOptions options)
    {
        return """
            <!DOCTYPE html>
            <html lang="en">
            <head>
            <meta charset = "UTF-8" >
            <meta http-equiv="X-UA-Compatible" content="IE=edge">
            <meta http-equiv="refresh" content="3">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>SPA client launch page</title>
            </head>
            <body>
            <style>
                @media (prefers-color-scheme: dark) {
                :root {
                    background: black;
                    color: gray;
                }
                }
            </style>
            <h1>Launching the SPA client...</h1>
            <p>This page will automatically refresh when the SPA client is ready.</p>
            </body>
            </html>
            """;
    }

    /// <summary>
    /// Custom request transformation
    /// </summary>
    private class SuppressOriginalHostTransformer : HttpTransformer
    {
        /// <summary>
        /// A callback that is invoked prior to sending the proxied request. All HttpRequestMessage
        /// fields are initialized except RequestUri, which will be initialized after the
        /// callback if no value is provided. The string parameter represents the destination
        /// URI prefix that should be used when constructing the RequestUri. The headers
        /// are copied by the base implementation, excluding some protocol headers like HTTP/2
        /// pseudo headers (":authority").
        /// </summary>
        /// <param name="httpContext">The incoming request.</param>
        /// <param name="proxyRequest">The outgoing proxy request.</param>
        /// <param name="destinationPrefix">The uri prefix for the selected destination server which can be used to create
        /// the RequestUri.</param>
        /// <param name="cancellationToken"></param>
        public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix, CancellationToken cancellationToken = default)
        {
            // Copy all request headers
            await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);

            // Suppress the original request header, use the one from the destination Uri.
            proxyRequest.Headers.Host = null;
        }
    }

}
