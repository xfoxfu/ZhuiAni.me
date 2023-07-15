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

    public record CreateAnimeDto(
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp
    );

    [HttpPost("animes")]
    public async Task<Anime> Create(CreateAnimeDto req)
    {
        var anime = new Anime
        {
            Bangumi = req.Bangumi,
            Target = req.Target,
            Regex = req.Regex,
            MatchGroup = new Anime.MatchGroups
            {
                Ep = req.MatchGroupEp
            }
        };
        Db.PikPakAnime.Add(anime);
        await Db.SaveChangesAsync();
        return anime;
    }

    [HttpGet("animes/{id}")]
    public async Task<Anime> Get(uint id)
    {
        return await Db.PikPakAnime.FindAsync(id) ?? throw new Exception($"Anime {id} not found.");
    }

    [HttpPost("animes/{id}")]
    public async Task<Anime> Update(uint id, CreateAnimeDto req)
    {
        var anime = await Db.PikPakAnime.FindAsync(id) ?? throw new Exception($"Anime {id} not found.");
        anime.Bangumi = req.Bangumi;
        anime.Target = req.Target;
        anime.Regex = req.Regex;
        anime.MatchGroup.Ep = req.MatchGroupEp;
        await Db.SaveChangesAsync();
        return anime;
    }

    [HttpDelete("animes/{id}")]
    public async Task Delete(uint id)
    {
        var anime = await Db.PikPakAnime.FindAsync(id) ?? throw new Exception($"Anime {id} not found.");
        Db.PikPakAnime.Remove(anime);
        await Db.SaveChangesAsync();
    }
}
