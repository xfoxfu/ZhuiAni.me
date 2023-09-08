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
    protected DbSet<PikPakJob> DbPikPakJob => Db.Set<PikPakJob>();

    public PikPakController(PikPakClient pikPakClient, ZAContext db)
    {
        PikPak = pikPakClient;
        Db = db;
    }

    public record JobDto(
        uint Id,
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp,
        bool Enabled,
        DateTimeOffset LastFetchedAt
    )
    {
        public JobDto(PikPakJob anime) : this(
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
    public async Task<IEnumerable<JobDto>> List()
    {
        return await DbPikPakJob
            .OrderByDescending(x => x.LastFetchedAt)
            .Select(a => new JobDto(a))
            .ToListAsync();
    }

    public record CreateJobDto(
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp
    );

    [HttpPost("jobs")]
    public async Task<JobDto> Create(CreateJobDto req)
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
        DbPikPakJob.Add(anime);
        await Db.SaveChangesAsync();
        return new JobDto(anime);
    }

    [HttpGet("jobs/{id}")]
    public async Task<JobDto> Get(uint id)
    {
        return new JobDto(
            await DbPikPakJob.FindAsync(id) ??
                throw new ZAError.PikPakJobNotFound(id)
        );
    }

    public record UpdateJobDto(
        uint Bangumi,
        string Target,
        string Regex,
        uint MatchGroupEp,
        bool Enabled
    );

    [HttpPost("jobs/{id}")]
    public async Task<JobDto> Update(uint id, UpdateJobDto req)
    {
        var anime = await DbPikPakJob.FindAsync(id) ??
            throw new ZAError.PikPakJobNotFound(id);
        anime.Bangumi = req.Bangumi;
        anime.Target = req.Target;
        anime.Regex = req.Regex;
        anime.MatchGroup = new PikPakJob.MatchGroups { Ep = req.MatchGroupEp };
        anime.Enabled = req.Enabled;
        await Db.SaveChangesAsync();
        return new JobDto(anime);
    }

    [HttpDelete("jobs/{id}")]
    public async Task Delete(uint id)
    {
        var anime = await DbPikPakJob.FindAsync(id) ??
            throw new ZAError.PikPakJobNotFound(id);
        DbPikPakJob.Remove(anime);
        await Db.SaveChangesAsync();
    }
}
