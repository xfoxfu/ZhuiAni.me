using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Get anime.
/// </summary>
[ApiController, Route("api/episodes")]
public class EpisodeController : ControllerBase
{
    public record EpisodeDto(
        uint Id,
        string Name,
        string Title
    );

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
