using System.Text;
using System.Text.Json;
using Me.Xfox.ZhuiAnime.External.BangumiMoe.Models;
using RestSharp;
using RocksDbSharp;

namespace Me.Xfox.ZhuiAnime.External.BangumiMoe;

public class Worker : IHostedService, IDisposable
{
  private readonly ILogger<Worker> _logger;
  private readonly RocksDb _db;
  private readonly RestClient _client;
  private Timer? _timerLatest = null;
  private Timer? _timerNextPage = null;

  public Worker(ILogger<Worker> logger, RocksDb db)
  {
    _logger = logger;
    _db = db;
    var options = new RestClientOptions
    {
      BaseUrl = new Uri("https://bangumi.moe/api/"),
      UserAgent = "xfoxfu/ZhuiAni.me (https://github.com/xfoxfu/ZhuiAni.me)",
    };
    _client = new(options);
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
    foreach (var torrent in response.Torrents)
    {
      _db.Put(torrent.Id, JsonSerializer.Serialize(torrent));
    }
    _logger.LogInformation("Saved latest page");
  }

  protected async void UpdateNextPage(object? state)
  {
    var lastPage = _db.HasKey("__next_page") ? Convert.ToUInt64(_db.Get("__next_page")) : 2;
    var lastPageResponse = await GetPageAsync(lastPage);
    if (!lastPageResponse.Torrents.Any() && lastPage > lastPageResponse.PageCount)
    {
      _logger.LogInformation("No more pages, stop updating next page");
      _timerNextPage?.Change(Timeout.Infinite, 0);
      return;
    }
    foreach (var torrent in lastPageResponse.Torrents)
    {
      _db.Put(torrent.Id, JsonSerializer.Serialize(torrent));
    }
    _logger.LogInformation("Saved last page: {page}", lastPage);
    _db.Put("__next_page", (lastPage + 1).ToString());
  }

  protected async Task<BangumiResponse> GetLatestAsync()
  {
    var request = new RestRequest($"torrent/latest");
    var response = await _client.GetAsync<BangumiResponse>(request) ?? throw new Exception("Bangumi responded with null");
    return response;
  }

  protected async Task<BangumiResponse> GetPageAsync(ulong page)
  {
    var request = new RestRequest($"torrent/page/{page}");
    var response = await _client.GetAsync<BangumiResponse>(request) ?? throw new Exception("Bangumi responded with null");
    return response;
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
