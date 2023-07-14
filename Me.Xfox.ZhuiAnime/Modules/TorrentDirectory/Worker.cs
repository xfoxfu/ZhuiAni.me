using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

public class Worker<S> : IHostedService, IDisposable where S : ISource
{
    private ILogger<Worker<S>> Logger { get; init; }
    private S Source { get; init; }
    private IServiceProvider ServiceProvider { get; init; }
    private Timer? Timer { get; set; }
    private TimeSpan IntervalBetweenPages { get; } = TimeSpan.FromSeconds(10);
    private uint FetchPageThreshold { get; } = 20;

    public Worker(ILogger<Worker<S>> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        Source = Activator.CreateInstance<S>() ??
            throw new Exception($"Failed to create instance of {nameof(S)}");
    }

    public Task StartAsync(CancellationToken ct)
    {
        var span = TimeSpan.FromMinutes(5);
        Timer = new Timer(UpdateData, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        Logger.LogInformation("Started timed service for {@Source} every {@Span}", Source.Name, span);
        return Task.CompletedTask;
    }

    protected async void UpdateData(object? state)
    {
        using var db = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ZAContext>();
        uint nextPage = 1;
        while (nextPage != 0)
        {
            bool hasNonExistent = await GetPage(nextPage, db);
            if (hasNonExistent)
            {
                Logger.LogInformation("{@Source} Page {@Page} has unsaved item, continuing", Source.Name, nextPage);
                nextPage += 1;
                if (nextPage > FetchPageThreshold)
                {
                    Logger.LogInformation("{@Source} Page {@Page} reached threshold, stopping", Source.Name, nextPage);
                    nextPage = 0;
                }
            }
            else
            {
                Logger.LogInformation("{@Source} Page {@Page} all items saved, stopping", Source.Name, nextPage);
                nextPage = 0;
            }
            await Task.Delay(IntervalBetweenPages);
        }
    }

    public async Task<bool> GetPage(uint page, ZAContext db)
    {
        var response = await Source.GetPageAsync(page);
        Logger.LogInformation("{@Source} Got page {@Page}", Source.Name, page);

        bool hasNonExistent = false;

        foreach (var item in response)
        {
            if (!await db.Torrent.Where(e => e.OriginSite == item.OriginSite && e.OriginId == item.OriginId).AnyAsync())
            {
                hasNonExistent = true;
                db.Torrent.Add(item);
            }
        }
        await db.SaveChangesAsync();

        return hasNonExistent;
    }

    public Task StopAsync(CancellationToken ct)
    {
        Logger.LogInformation("Stopped timed service for {@Source}.", Source.Name);

        Timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
