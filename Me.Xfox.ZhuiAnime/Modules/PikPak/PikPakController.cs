using Me.Xfox.ZhuiAnime.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

/// <summary>
/// Get items.
/// </summary>
[ApiController, Route("api/modules/pikpak")]
public class PikPakController : ControllerBase
{
    protected PikPakClient PikPak { get; init; }
    public PikPakController(PikPakClient pikPakClient)
    {
        PikPak = pikPakClient;
    }

    public record DownloadDto(string Url, string Path);

    [HttpPost("download")]
    public async Task<string> Download(DownloadDto req)
    {
        var path = req.Path.Split("/").Where(x => !string.IsNullOrWhiteSpace(x));
        var file = await PikPak.DownloadLink(req.Url, path);
        return file.Id;
    }
}
