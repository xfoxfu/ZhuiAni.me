using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

/// <summary>
/// Operate links under an item.
/// </summary>
[ApiController, Route("api/modules/torrent_directory")]
public class TorrentController : ControllerBase
{
    private ZAContext DbContext { get; init; }
    private IServiceProvider Services { get; init; }

    public TorrentController(ZAContext dbContext, IServiceProvider services)
    {
        DbContext = dbContext;
        Services = services;
    }

    public record TorrentDto(
        uint Id,
        string OriginSite,
        string OriginId,
        string Title,
        DateTimeOffset PublishedAt,
        string? LinkTorrent,
        string? LinkMagnet
    );

    [HttpGet("torrents")]
    public async Task<ActionResult<IEnumerable<TorrentDto>>> ListAsync(
        [FromQuery] string? query,
        [FromQuery] int? count,
        [FromQuery] DateTimeOffset? until)
    {
        var linq = DbContext.Torrent.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            linq = linq.Where(t => Regex.IsMatch(t.Title, query));
        }
        if (until != null)
        {
            var untilUtc = until?.ToUniversalTime();
            linq = linq.Where(t => t.PublishedAt < untilUtc);
        }

        return await linq
            .OrderByDescending(t => t.PublishedAt)
            .Take(Math.Min(count ?? 20, 100))
            .Select(t => new TorrentDto(
                t.Id,
                t.OriginSite,
                t.OriginId,
                t.Title,
                t.PublishedAt,
                t.LinkTorrent,
                t.LinkMagnet
            ))
            .ToListAsync();
    }

    [HttpPost("providers/{name}/fetch/{page}")]
    public async Task<IActionResult> FetchPage([FromRoute] string name, [FromRoute] uint page)
    {
        if (name == "bangumi.moe")
        {
            var worker = Services.GetService<Worker<Sources.BangumiSource>>()!;
            var hasNE = await worker.GetPage(page, DbContext);
            return Ok(new { HasNewItems = hasNE });
        }
        else if (name == "acg.rip")
        {
            var worker = Services.GetService<Worker<Sources.AcgRipSource>>()!;
            var hasNE = await worker.GetPage(page, DbContext);
            return Ok(new { HasNewItems = hasNE });
        }
        else
        {
            throw new Exception("unknown source, please use bangumi.moe or acg.rip");
        }
    }
}
