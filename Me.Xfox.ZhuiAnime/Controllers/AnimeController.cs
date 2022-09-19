using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers
{
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
        public record Anime(
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
        public async Task<IEnumerable<Anime>> ListAsync([FromQuery(Name = "include_image")] bool includeImage = false)
        {
            return await DbContext.Anime
                .Select(a => new Anime(a.Id, a.Title, a.BangumiLink, includeImage ? a.ImageBase64 : null))
                .ToListAsync();
        }

        /// <summary>
        /// Detailed anime information.
        /// </summary>
        /// <param name="Id">id</param>
        /// <param name="Title">original title</param>
        /// <param name="BangumiLink">link to bgm.tv subject</param>
        /// <param name="ImageBase64">key vision image (zero byte if not exist), base64 encoded</param>
        public record AnimeDetailed(
            uint Id,
            string Title,
            Uri BangumiLink,
            string ImageBase64
        );

        [HttpGet("{id}")]
        public async Task<AnimeDetailed> GetAsync([FromRoute] uint id)
        {
            var anime = await DbContext.Anime
                .Where(a => a.Id == id)
                .Select(a => new AnimeDetailed(
                    a.Id,
                    a.Title,
                    a.BangumiLink,
                    a.ImageBase64))
                .SingleOrDefaultAsync();
            if (anime == null) throw new ZhuiAnimeError.AnimeNotFound(id);
            return anime;
        }

        public record Episode(
            uint Id,
            string Name,
            string Title
        );

        [HttpGet("{id}/episodes")]
        public async Task<IEnumerable<Episode>> GetEpisodesAsync([FromRoute] uint id)
        {
            return await DbContext.Episode
                .Where(e => e.AnimeId == id)
                .Select(e => new Episode(e.Id, e.Name, e.Title))
                .ToListAsync();
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
}
