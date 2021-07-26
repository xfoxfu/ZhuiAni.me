using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Controllers
{
    public record Subscription(string N, string E, string G, string R, string S, string T, string L);
    [ApiController, Route("/api/subscriptions")]
    public class SubscriptionsController : ControllerBase
    {
        [HttpGet, Route("")]
        public List<Subscription> Get()
        {
            var subscriptions = new[]
            {
                new Subscription("现实主义勇者的王国再建记", "04", "NC-Raws", "1080p", "WebDL", "MKV", "zh-Hans"),
                new Subscription("现实主义勇者的王国再建记", "04", "NC-Raws", "1080p", "WebDL", "MKV", "zh-Hans"),
                new Subscription("现实主义勇者的王国再建记", "04", "NC-Raws", "1080p", "WebDL", "MKV", "zh-Hans"),
                new Subscription("现实主义勇者的王国再建记", "04", "NC-Raws", "1080p", "WebDL", "MKV", "zh-Hans"),
            };


            return subscriptions.ToList();
        }
    }
}
