using System.Text.Json;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using RocksDbSharp;

namespace Me.Xfox.ZhuiAnime.External.AcgRip;

public class Worker : IHostedService, IDisposable
{
  private readonly ILogger<Worker> _logger;
  private readonly RocksDb _db;
  private Timer? _timerLatest = null;
  private Timer? _timerNextPage = null;

  public Worker(ILogger<Worker> logger, RocksDb db)
  {
    _logger = logger;
    _db = db;
  }

  public Task StartAsync(CancellationToken ct)
  {
    _timerLatest = new Timer(UpdateLatestPage, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    _timerNextPage = new Timer(UpdateNextPage, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

    return Task.CompletedTask;
  }

  protected async void UpdateLatestPage(object? state)
  {
    var response = await GetLatestAsync();
    foreach (var torrent in response)
    {
      _db.Put(torrent.Guid, JsonSerializer.Serialize(torrent));
    }
    _logger.LogInformation("Saved latest page");
  }

  protected async void UpdateNextPage(object? state)
  {
    var lastPage = _db.HasKey("__next_page") ? Convert.ToUInt64(_db.Get("__next_page")) : 1;
    var lastPageResponse = await GetPageAsync(lastPage);
    if (!lastPageResponse.Any() && lastPage > 8550)
    {
      _logger.LogInformation("No more pages, stop updating next page");
      _timerNextPage?.Change(Timeout.Infinite, 0);
      return;
    }
    foreach (var torrent in lastPageResponse)
    {
      _db.Put(torrent.Guid, JsonSerializer.Serialize(torrent));
    }
    _logger.LogInformation("Saved last page: {page}", lastPage);
    _db.Put("__next_page", (lastPage + 1).ToString());
  }

  protected async Task<IEnumerable<BangumiTorrent>> GetLatestAsync()
  {
    var feed = await FeedReader.ReadAsync(
      "https://acg.rip/.xml",
      userAgent: "xfoxfu/ZhuiAni.me (https://github.com/xfoxfu/ZhuiAni.me)");
    return feed.Items.Select(ConvertFeedItem).ToList();
  }

  protected async Task<IEnumerable<BangumiTorrent>> GetPageAsync(ulong page)
  {
    var feed = await FeedReader.ReadAsync(
      $"https://acg.rip/page/{page}.xml",
      userAgent: "xfoxfu/ZhuiAni.me (https://github.com/xfoxfu/ZhuiAni.me)");
    return feed.Items.Select(ConvertFeedItem).ToList();
  }

  protected BangumiTorrent ConvertFeedItem(FeedItem item)
  {
    return new BangumiTorrent
    {
      Title = item.Title,
      Description = item.Description,
      PubDate = item.PublishingDateString,
      Link = item.Link,
      Guid = item.Id,
      Enclosure = (item.SpecificItem as Rss20FeedItem)?.Enclosure.Url,
    };
  }

  public Task StopAsync(CancellationToken ct)
  {
    _logger.LogInformation("Timed Hosted Service is stopping.");

    _timerLatest?.Change(Timeout.Infinite, 0);
    _timerNextPage?.Change(Timeout.Infinite, 0);

    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _timerLatest?.Dispose();
    _timerNextPage?.Dispose();
    GC.SuppressFinalize(this);
  }
}
