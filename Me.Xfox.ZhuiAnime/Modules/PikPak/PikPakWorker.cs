using System.Text.RegularExpressions;
using Me.Xfox.ZhuiAnime.Modules.Bangumi;
using Microsoft.Extensions.Options;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class PikPakWorker : IHostedService, IDisposable
{
    protected ILogger<PikPakWorker> Logger { get; init; }

    protected IOptionsMonitor<PikPakClient.Option> Options { get; set; }

    protected IServiceScopeFactory Services { get; init; }

    protected PikPakClient Client { get; init; }

    protected Timer? Timer { get; set; }

    protected TimeSpan IntervalBetweenFetch => Options.CurrentValue.IntervalBetweenFetch;

    public PikPakWorker(
        ILogger<PikPakWorker> logger,
        IOptionsMonitor<PikPakClient.Option> options,
        IServiceScopeFactory services,
        PikPakClient client)
    {
        Logger = logger;
        Options = options;
        Services = services;
        Client = client;
    }

    public Task StartAsync(CancellationToken ct)
    {
        var span = Options.CurrentValue.IntervalBetweenFetch;
        Timer = new Timer(TimerWork, null, TimeSpan.Zero, span);
        Logger.LogInformation("Started PikPak service every {@Span}", span);
        return Task.CompletedTask;
    }

    protected async void TimerWork(object? state)
    {
        try
        {
            await UpdateData();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error while updating data.");
        }
    }

    protected async Task UpdateData()
    {
        using var scope = Services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<ZAContext>();
        var bangumiService = scope.ServiceProvider.GetRequiredService<BangumiService>();

        var configs = await db.PikPakJob.Where(a => a.Enabled).ToListAsync();
        foreach (var config in configs)
        {
            Logger.LogInformation("Updating anime {@AnimeId}", config.Id);
            var torrents = await db.Torrent
                .Where(t => t.OriginSite == "https://bangumi.moe")
                .Where(t => t.PublishedAt > config.LastFetchedAt)
                .Where(t => Regex.IsMatch(t.Title, config.Regex))
                .OrderByDescending(t => t.PublishedAt)
                .ToListAsync();

            Models.Item bangumi;
            if (torrents.Count > 0)
            {
                var imported = await bangumiService.ImportSubject((int)config.Bangumi);
                bangumi = (await db.Item.FindAsync(imported))!;
            }
            else
            {
                var uri = new Uri($"https://bgm.tv/subject/{config.Bangumi}");
                var existing = await db.Link
                        .Include(l => l.Item)
                        .FirstOrDefaultAsync(l => l.Address == uri);
                bangumi = existing?.Item ??
                    (await db.Item.FindAsync(await bangumiService.ImportSubject((int)config.Bangumi)))!;
            }

            List<Models.Link> links = new();
            foreach (var torrent in torrents)
            {
                var link = await Client.ImportLink(config, torrent, db, bangumi);
                links.Add(link);

                Logger.LogInformation("Imported torrent {@TorrentId} to anime {@AnimeId}", torrent.Id, config.Id);
            }

            if (links.Count > 0)
            {
                using var tx = db.Database.BeginTransaction();
                db.Link.AddRange(links);
                config.LastFetchedAt = torrents.Select(t => t.PublishedAt).Max();
                await db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            Logger.LogInformation("Updated anime {@AnimeId}, {@Count} new items.", config.Id, links.Count);
        }
    }

    public Task StopAsync(CancellationToken ct)
    {
        Logger.LogInformation("Stopped PikPak service.");
        Timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
