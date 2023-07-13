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
        uint Id,
        string OriginSite,
        string OriginId,
        string Title,
        DateTimeOffset PublishedAt,
        string? LinkTorrent,
        string? LinkMagnet
    );

    [HttpGet("torrents")]
    public async Task<ActionResult<IEnumerable<TorrentDto>>> ListAsync([FromQuery] string? query, [FromQuery] int? count)
    {
        var baseQuery = DbContext.Torrent
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
            ));

        List<TorrentDto> torrents;
        if (string.IsNullOrEmpty(query))
        {
            torrents = await baseQuery.ToListAsync();
        }
        else
        {
            torrents = await baseQuery.Where(t => Regex.IsMatch(t.Title, query)).ToListAsync();
        }

        return torrents;
    }
}
