using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers
{
    [ApiController, Route("api/anime")]
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
        /// Get all anime.
        /// </summary>
        /// <returns>List of anime. Episodes and links are not returned.</returns>
        [HttpGet]
        public async Task<List<Models.Anime>> GetAsync()
        {
            return await DbContext.Anime.ToListAsync();
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
