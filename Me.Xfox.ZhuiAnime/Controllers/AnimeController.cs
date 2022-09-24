using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Models;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Get anime.
/// </summary>
[ApiController, Route("api/animes")]
public class AnimeController : ControllerBase
{
    private ZAContext DbContext { get; init; }
    private BangumiClient Bangumi { get; init; }

    public AnimeController(ZAContext dbContext, BangumiClient client)
    {
        DbContext = dbContext;
        Bangumi = client;
    }

    /// <summary>
    /// Anime information.
    /// </summary>
    /// <param name="Id">id</param>
    /// <param name="Title">original title</param>
    /// <param name="BangumiLink">link to bgm.tv subject</param>
    /// <param name="ImageBase64">key vision image (if include_image), base64 encoded</param>
    public record AnimeDto(
        uint Id,
        string Title,
        Uri BangumiLink,
        [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            string? ImageBase64
    );

    /// <summary>
    /// Get all animes.
    /// </summary>
    /// <param name="includeImage">whether include key vision image in result</param>
    /// <returns>List of anime.</returns>
    [HttpGet]
    public async Task<IEnumerable<AnimeDto>> ListAsync([FromQuery(Name = "include_image")] bool includeImage = false)
    {
        return await DbContext.Anime
            .Select(a => new AnimeDto(a.Id, a.Title, a.BangumiLink, includeImage ? a.ImageBase64 : null))
            .ToListAsync();
    }

    /// <summary>
    /// Detailed anime information.
    /// </summary>
    /// <param name="Id">id</param>
    /// <param name="Title">original title</param>
    /// <param name="BangumiLink">link to bgm.tv subject</param>
    /// <param name="ImageBase64">key vision image (zero byte if not exist), base64 encoded</param>
    public record AnimeDetailedDto(
        uint Id,
        string Title,
        Uri BangumiLink,
        string ImageBase64
    );

    [HttpGet("{anime}")]
    public AnimeDetailedDto Get(Anime anime)
    {
        return new AnimeDetailedDto(
            anime.Id,
            anime.Title,
            anime.BangumiLink,
            anime.ImageBase64
        );
    }

    public record ImportRequest(int Id);

    /// <summary>
    /// Import anime from bgm.tv.
    /// </summary>
    [HttpPost("import_bangumi")]
    public async Task ImportAsync(ImportRequest req)
    {
        await Bangumi.SubjectImportToAnimeAsync(req.Id);
    }
}
