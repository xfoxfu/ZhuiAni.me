using System.Runtime.Serialization;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkDto = Me.Xfox.ZhuiAnime.Controllers.ItemLinkController.LinkDto;
using static Me.Xfox.ZhuiAnime.Models.Link.CommonAnnotations;
using System.Text.RegularExpressions;
using Me.Xfox.ZhuiAnime.Modules.Bangumi;

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
    protected BangumiService Bangumi { get; init; }

    public PikPakController(PikPakClient pikPakClient, ZAContext db, BangumiService bangumi)
    {
        PikPak = pikPakClient;
        Db = db;
        Bangumi = bangumi;
    }

    public record JobDto(
        Ulid Id,
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
    public async Task<JobDto> Get(Ulid id)
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
    public async Task<JobDto> Update(Ulid id, UpdateJobDto req)
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
    public async Task Delete(Ulid id)
    {
        var anime = await DbPikPakJob.FindAsync(id) ??
            throw new ZAError.PikPakJobNotFound(id);
        DbPikPakJob.Remove(anime);
        await Db.SaveChangesAsync();
    }

    public record ListFolderDto
    {
        public required string Path { get; set; }
    }

    public record FileDto
    {
        public required string Name { get; set; }
        public required FileType Type { get; set; }
    }

    public enum FileType
    {
        [EnumMember(Value = "folder")]
        Folder,
        [EnumMember(Value = "file")]
        File,
    }

    [HttpPost("list_folder")]
    public async Task<IEnumerable<FileDto>> ListFolder(ListFolderDto req)
    {
        await PikPak.EnsureLogin();
        var folder = await PikPak.ResolveFolderNoCreate(req.Path.Split("/").Where(x => !string.IsNullOrWhiteSpace(x)));
        if (folder == null) return new List<FileDto>();
        var files = await PikPak.List(folder.Id);
        return files
            .Where(x => !x.Trashed)
            .Select(x => new FileDto
            {
                Name = x.Name,
                Type = x.Kind switch
                {
                    "drive#folder" => FileType.Folder,
                    "drive#file" => FileType.File,
                    _ => throw new Exception($"unexpected file type {x.Kind}")
                },
            });
    }

    public record ImportFolderDto
    {
        public required string Path { get; set; }
        public required string Regex { get; set; }
        public required uint MatchGroupEp { get; set; }
        public required uint Bangumi { get; set; }
    }

    [HttpPost("import_folder")]
    public async Task<IEnumerable<LinkDto>> ImportFolder(ImportFolderDto req)
    {
        var links = await PikPak.ImportFolder(Bangumi, Db, req.Path, req.Regex, req.MatchGroupEp, req.Bangumi);
        return links.Select(x => new LinkDto(x));
    }
}
