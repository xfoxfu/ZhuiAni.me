using System.Text.Json;
using Me.Xfox.ZhuiAnime.External.BangumiMoe.Models;
using Microsoft.AspNetCore.Mvc;
using RocksDbSharp;

namespace Me.Xfox.ZhuiAnime.External.BangumiMoe;

[ApiController]
[Route("")]
public class BangumiController : ControllerBase
{
  private readonly ILogger<BangumiController> _logger;
  private readonly RocksDb _db;

  public BangumiController(ILogger<BangumiController> logger, RocksDb db)
  {
    _logger = logger;
    _db = db;
  }

  [HttpGet("item_count")]
  public ulong GetItemCount()
  {
    return Convert.ToUInt64(_db.GetProperty("rocksdb.estimate-num-keys"));
  }

  [HttpGet("next_page")]
  public ulong GetLastPage()
  {
    return Convert.ToUInt64(_db.Get("__next_page"));
  }

  [HttpGet("{id}")]
  public BangumiTorrent GetTorrent(string id)
  {
    return JsonSerializer.Deserialize<BangumiTorrent>(_db.Get(id)) ?? throw new Exception("invalid information");
  }

  [HttpGet("keys")]
  public IEnumerable<BangumiTorrent> GetKeys(
    [FromQuery(Name = "last")] string? last,
    [FromQuery(Name = "count")] uint? maxCount)
  {
    using var it = _db.NewIterator();
    if (last != null)
    {
      it.Seek(last);
      it.Prev();
    }
    else
    {
      it.SeekToLast();
    }
    ulong count = 0;
    while (it.Valid())
    {
      var value = it.StringKey() != "__next_page" ?
        JsonSerializer.Deserialize<BangumiTorrent>(it.StringValue()) : null;
      if (value != null) yield return value;
      count += 1;
      if (count >= Math.Min(maxCount ?? 10, 100))
      {
        break;
      }
      it.Prev();
    }
  }
}
