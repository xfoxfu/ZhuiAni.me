using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Controllers
{
    [ApiController, Route("/api/anime")]
    public class AnimeController : ControllerBase
    {
        [HttpGet, Route("")]
        public List<string[]> Get()
        {
            var anime = new[]
            {
                ("名侦探柯南", "https://mikanani.me/images/Bangumi/201310/91d95f43.jpg"),
                ("海贼王", "https://mikanani.me/images/Bangumi/201310/0aa598c7.jpg"),
                ("火影忍者 博人传之火影次世代", "https://mikanani.me/images/Bangumi/201704/e46ad033.jpg"),
                ("宝可梦 旅途", "https://mikanani.me/images/Bangumi/201911/0b25e1cd.jpg"),
                ("数码兽大冒险：", "https://mikanani.me/images/Bangumi/202004/2e66e770.jpg"),
                ("王者天下 第三季", "https://mikanani.me/images/Bangumi/202004/5988c4af.jpg"),
                ("舞伎家的料理人", "https://mikanani.me/images/Bangumi/202102/d74196db.jpg"),
                ("Tropical-Rouge！光之美少女", "https://mikanani.me/images/Bangumi/202102/f3a3a47c.jpg"),
                ("致不灭的你", "https://mikanani.me/images/Bangumi/202104/98a02bea.jpg"),
                ("甜梦猫 MIX!", "https://mikanani.me/images/Bangumi/202104/8418c9a0.jpg"),
                ("指尖传出的真挚热情2-恋人是消防员-", "https://mikanani.me/images/Bangumi/202106/efe4ab77.jpg"),
                ("死神少爷与黑女仆", "https://mikanani.me/images/Bangumi/202107/cbc5d713.jpg")
            };

            return anime.Select(i => new[] { i.Item1, i.Item2 }).ToList();
        }
    }
}
