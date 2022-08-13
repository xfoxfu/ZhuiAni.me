using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers
{
    [ApiController, Route("api/anime")]
    public class AnimeController : ControllerBase
    {
        public AnimeController(ZAContext dbContext)
        {
            DbContext = dbContext;
        }

        private ZAContext DbContext { get; init; }

        [HttpGet("")]
        public async Task<List<Models.Anime>> GetAsync()
        {
            return await DbContext.Anime.ToListAsync();
        }
    }
}
