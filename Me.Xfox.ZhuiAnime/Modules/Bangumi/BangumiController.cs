using Me.Xfox.ZhuiAnime.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

/// <summary>
/// Functionality related to https://bgm.tv .
/// </summary>
[ApiController, Route("api/modules/bangumi")]
public class BangumiController : ControllerBase
{
    protected BangumiService Bangumi { get; init; }
    public BangumiController(BangumiService bangumi)
    {
        Bangumi = bangumi;
    }

    public record ImportSubjectDto
    {
        /// <summary>
        /// bangumi subject ID
        /// </summary>
        public required int Id { get; set; }
    }

    /// <summary>Import Subject</summary>
    /// <remarks>
    /// Import a subject from https://bgm.tv .
    /// </remarks>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("import_subject")]
    public async Task<ItemController.ItemDto> ImportSubject(ImportSubjectDto req)
    {
        var id = req.Id;
        var item = await Bangumi.ImportSubject(id);
        return new ItemController.ItemDto(item);
    }

    public record SearchRequestDto
    {
        public required string Query { get; set; }
    }

    public record SearchResultItemDto
    {
        public required uint Id { get; set; }
        public required string Name { get; set; }
        public required string NameCn { get; set; }
    }

    [HttpPost("search_subject")]
    public async Task<IEnumerable<SearchResultItemDto>> SearchSubject(SearchRequestDto req)
    {
        return await Bangumi.SearchAsync(req.Query)
            .Select(x => new SearchResultItemDto
            {
                Id = (uint)(x.Id ?? 0),
                Name = x.Name,
                NameCn = x.NameCn,
            })
            .ToListAsync();
    }
}
