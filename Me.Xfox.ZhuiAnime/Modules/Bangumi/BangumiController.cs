using Me.Xfox.ZhuiAnime.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

/// <summary>
/// Get items.
/// </summary>
[ApiController, Route("api/modules/bangumi")]
public class BangumiController : ControllerBase
{
    protected BangumiService Bangumi { get; init; }
    public BangumiController(BangumiService bangumi)
    {
        Bangumi = bangumi;
    }

    public record ImportSubjectDto(int Id);

    [HttpPost("import_subject")]
    public async Task<ItemController.ItemDto> ImportSubject(ImportSubjectDto req)
    {
        var id = req.Id;
        var item = await Bangumi.ImportSubject(id);
        return new ItemController.ItemDto(item);
    }
}
