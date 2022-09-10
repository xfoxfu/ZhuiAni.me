using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers
{
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

        public record Anime(
            uint Id,
            string Title,
            Uri BangumiLink
        );

        /// <summary>
        /// Get all anime.
        /// </summary>
        /// <returns>List of anime. Episodes and links are not returned.</returns>
        [HttpGet]
        public async Task<IEnumerable<Anime>> ListAsync()
        {
            return await DbContext.Anime
                .Select(a => new Anime(a.Id, a.Title, a.BangumiLink))
                .ToListAsync();
        }

        public record AnimeDetailed(
            uint Id,
            string Title,
            Uri BangumiLink,
            string? Image
        );

        [HttpGet("{id}")]
        public async Task<AnimeDetailed> GetAsync([FromRoute] uint id)
        {
            return await DbContext.Anime
                .Where(a => a.Id == id)
                .Select(a => new AnimeDetailed(
                    a.Id,
                    a.Title,
                    a.BangumiLink,
                    a.Image != null ? Convert.ToBase64String(a.Image) : null))
                .SingleAsync();
        }

        public record Episode(
            uint Id,
            string Title
        );

        [HttpGet("{id}/episodes")]
        public async Task<IEnumerable<Episode>> GetEpisodesAsync([FromRoute] uint id)
        {
            return await DbContext.Episode
                .Where(e => e.AnimeId == id)
                .Select(e => new Episode(e.Id, e.Title))
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
