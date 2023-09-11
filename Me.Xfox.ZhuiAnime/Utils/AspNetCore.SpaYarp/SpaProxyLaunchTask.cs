namespace AspNetCore.SpaYarp;

public class SpaProxyLaunchTask : BackgroundService
{
    protected readonly IServiceProvider Services;
    protected readonly ILogger<SpaProxyLaunchTask> _logger;

    public SpaProxyLaunchTask(IServiceProvider services,
        ILogger<SpaProxyLaunchTask> logger)
    {
        Services = services;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var manager = Services.GetRequiredService<SpaProxyLaunchManager>();
        manager.StartInBackground(stoppingToken);
        return Task.CompletedTask;
    }
}
