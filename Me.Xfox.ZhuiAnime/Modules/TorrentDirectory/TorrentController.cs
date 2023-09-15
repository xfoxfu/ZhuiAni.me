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

    public TorrentController(ZAContext dbContext)
    {
        DbContext = dbContext;
    }

    public record TorrentDto(
        Ulid Id,
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
        var linq = DbContext.Set<Torrent>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            DbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(10));
            linq = linq.Where(t => Regex.IsMatch(t.Title, query, RegexOptions.IgnoreCase | RegexOptions.NonBacktracking));
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
                t.IdV2,
                t.OriginSite,
                t.OriginId,
                t.Title,
                t.PublishedAt,
                t.LinkTorrent,
                t.LinkMagnet
            ))
            .ToListAsync();
    }

    public record FetchPageResponseDto(bool HasNewItems);

    [HttpPost("providers/{name}/fetch/{page}")]
    public async Task<FetchPageResponseDto> FetchPage([FromRoute] string name, [FromRoute] uint page)
    {
        if (name == "bangumi.moe")
        {
            ISource source = Activator.CreateInstance<Sources.BangumiSource>();
            var hasNE = await source.GetPage(page, DbContext);
            return new(HasNewItems: hasNE);
        }
        else if (name == "acg.rip")
        {
            ISource source = Activator.CreateInstance<Sources.AcgRipSource>();
            var hasNE = await source.GetPage(page, DbContext);
            return new(HasNewItems: hasNE);
        }
        else
        {
            throw new Exception("unknown source, please use bangumi.moe or acg.rip");
        }
    }
}
