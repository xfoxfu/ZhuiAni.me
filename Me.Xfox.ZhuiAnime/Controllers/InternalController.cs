using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Controllers;

[ApiController, Route("api/__internal")]
public class InternalController : ControllerBase
{
    [Route("not_found")]
    public void EndpointNotFound()
    {
        throw new ZhuiAnimeError.EndpointNotFound();
    }
}
