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
public class EpisodeController : ControllerBase
{
    private ZAContext DbContext { get; init; }

    public EpisodeController(ZAContext dbContext)
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
            .Select(e => new EpisodeDto(e.Id, e.Name, e.Title))
            .ToListAsync();
    }

    [HttpGet("{episode}")]
    public EpisodeDto GetEpisode(Anime anime, Episode episode)
    {
        if (anime.Id != episode.AnimeId)
        {
            throw new ZhuiAnimeError.EpisodeNotBelongToAnime(episode.Id, anime.Id);
        }

        return new EpisodeDto(
            episode.Id,
            episode.Name,
            episode.Title
        );
    }
}
