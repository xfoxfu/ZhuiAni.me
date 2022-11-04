using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Get anime.
/// </summary>
[ApiController, Route("api/animes/{anime}/episodes")]
public class AnimeEpisodeController : ControllerBase
{
    private ZAContext DbContext { get; init; }

    public AnimeEpisodeController(ZAContext dbContext)
    {
        DbContext = dbContext;
    }

    public record EpisodeDto(
        uint Id,
        string Name,
        string Title
    );

    [HttpGet]
    public async Task<IEnumerable<EpisodeDto>> GetEpisodesAsync(Anime anime)
    {
        return await DbContext.Entry(anime)
            .Collection(a => a.Episodes!)
            .Query()
            .OrderBy(e => e.Name)
            .Select(e => new EpisodeDto(e.Id, e.Name, e.Title))
            .ToListAsync();
    }

    [HttpGet("{episode}")]
    public EpisodeDto GetEpisode(Episode episode)
    {
        return new EpisodeDto(
            episode.Id,
            episode.Name,
            episode.Title
        );
    }
}
