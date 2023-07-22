using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

/// <summary>
/// Get items.
/// </summary>
[ApiController, Route("api/modules/pikpak")]
public class PikPakController : ControllerBase
{
    protected PikPakClient PikPak { get; init; }

    protected ZAContext Db { get; init; }

    public PikPakController(PikPakClient pikPakClient, ZAContext db)
    {
        PikPak = pikPakClient;
        Db = db;
    }

    public record AnimeDto(
        uint Id,
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp,
        bool Enabled,
        DateTimeOffset LastFetchedAt
    )
    {
        public AnimeDto(PikPakJob anime) : this(
            anime.Id,
            anime.Bangumi,
            anime.Target,
            anime.Regex,
            anime.MatchGroup.Ep,
            anime.Enabled,
            anime.LastFetchedAt
        )
        { }
    }

    [HttpGet("jobs")]
    public async Task<IEnumerable<AnimeDto>> List()
    {
        return await Db.PikPakJob
            .OrderByDescending(x => x.LastFetchedAt)
            .Select(a => new AnimeDto(a))
            .ToListAsync();
    }

    public record CreateAnimeDto(
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp
    );

    [HttpPost("jobs")]
    public async Task<AnimeDto> Create(CreateAnimeDto req)
    {
        var anime = new PikPakJob
        {
            Bangumi = req.Bangumi,
            Target = req.Target,
            Regex = req.Regex,
            MatchGroup = new PikPakJob.MatchGroups
            {
                Ep = req.MatchGroupEp
            }
        };
        Db.PikPakJob.Add(anime);
        await Db.SaveChangesAsync();
        return new AnimeDto(anime);
    }

    [HttpGet("jobs/{id}")]
    public async Task<AnimeDto> Get(uint id)
    {
        return new AnimeDto(
            await Db.PikPakJob.FindAsync(id) ?? throw new Exception($"Anime {id} not found.")
        );
    }

    public record UpdateAnimeDto(
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp,
        bool Enabled
    );

    [HttpPost("jobs/{id}")]
    public async Task<AnimeDto> Update(uint id, UpdateAnimeDto req)
    {
        var anime = await Db.PikPakJob.FindAsync(id) ?? throw new Exception($"Anime {id} not found.");
        anime.Bangumi = req.Bangumi;
        anime.Target = req.Target;
        anime.Regex = req.Regex;
        anime.MatchGroup = new PikPakJob.MatchGroups { Ep = req.MatchGroupEp };
        anime.Enabled = req.Enabled;
        await Db.SaveChangesAsync();
        return new AnimeDto(anime);
    }

    [HttpDelete("jobs/{id}")]
    public async Task Delete(uint id)
    {
        var anime = await Db.PikPakJob.FindAsync(id) ?? throw new Exception($"Anime {id} not found.");
        Db.PikPakJob.Remove(anime);
        await Db.SaveChangesAsync();
    }
}
