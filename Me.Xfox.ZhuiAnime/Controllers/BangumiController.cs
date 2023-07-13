using Me.Xfox.ZhuiAnime.External.Bangumi;
using Me.Xfox.ZhuiAnime.External.Bangumi.Models;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Get items.
/// </summary>
[ApiController, Route("api/external/bangumi")]
public class BangumiController : ControllerBase
{
    protected ZAContext DbContext { get; init; }
    protected BangumiApi BgmApi { get; init; }

    public BangumiController(ZAContext dbContext, BangumiApi bgmApi)
    {
        DbContext = dbContext;
        BgmApi = bgmApi;
    }

    public record ImportSubjectDto(int Id);

    [HttpPost("import_subject")]
    public async Task<ItemController.ItemDto> ImportSubject(ImportSubjectDto req)
    {
        var id = req.Id;

        Category category;
        using (var tx = DbContext.Database.BeginTransaction())
        {
            var anime = await DbContext.Category.Where(a => a.Title == "アニメ").FirstOrDefaultAsync();
            if (anime == null)
            {
                anime = new();
                DbContext.Category.Add(anime);
            }

            anime.Title = "アニメ";

            await DbContext.SaveChangesAsync();
            await tx.CommitAsync();
            category = anime;
        }

        var bgmAnime = await BgmApi.GetSubjectAsync(id);
        if (bgmAnime.Type != SubjectType.Anime) throw new Exception($"subject {id} is not anime");

        Models.Item item;
        using (var tx = DbContext.Database.BeginTransaction())
        {
            var address = new Uri($"https://bgm.tv/subject/{id}");

            var anime = await DbContext.Item.Where(a => a.Title == bgmAnime.Name).FirstOrDefaultAsync()
                ?? await DbContext.Link.Where(l => l.Address == address).Select(l => l.Item).FirstOrDefaultAsync();
            if (anime == null)
            {
                anime = new();
                DbContext.Item.Add(anime);
            }

            anime.Title = bgmAnime.Name;
            anime.CategoryId = category.Id;

            await DbContext.SaveChangesAsync();
            item = anime;

            var link = await DbContext.Link.Where(l => l.ItemId == item.Id && l.Address == address)
                .FirstOrDefaultAsync();
            if (link == null)
            {
                link = new();
                DbContext.Link.Add(link);
            }
            link.ItemId = item.Id;
            link.Address = address;
            link.MimeType = "text/html;kind=bgm.tv";

            await DbContext.SaveChangesAsync();
            await tx.CommitAsync();
        }

        int episodeNameLength = Convert.ToInt32(Math.Ceiling(Math.Log10(bgmAnime.TotalEpisodes + 1)));
        string episodeNameFormat = $"{new('0', episodeNameLength)}.###";

        var episodes = BgmApi.GetEpisodesAsync(id);
        await foreach (var bgmEpisode in episodes)
        {
            if (bgmEpisode.Type != Episode.EpisodeType.Origin && bgmEpisode.Type != Episode.EpisodeType.SP)
            {
                continue;
            }

            var address = new Uri($"https://bgm.tv/ep/{bgmEpisode.Id}");
            var name = (bgmEpisode.Sort ?? 0).ToString(episodeNameFormat);
            if (bgmEpisode.Type == Episode.EpisodeType.SP) name = $"SP{name}";

            using var tx = DbContext.Database.BeginTransaction();

            var episode = await DbContext.Item.Where(a => a.Title.StartsWith(name) && a.ParentItem == item).FirstOrDefaultAsync()
                ?? await DbContext.Link.Where(l => l.Address == address).Select(l => l.Item).FirstOrDefaultAsync();
            if (episode == null)
            {
                episode = new();
                DbContext.Item.Add(episode);
            }

            episode.Title = string.IsNullOrEmpty(bgmEpisode.Name) ? name : $"{name} - {bgmEpisode.Name}";
            episode.Category = category;
            episode.ParentItem = item;
            episode.Annotations = new Dictionary<string, string>(episode.Annotations)
            {
                ["https://bgm.tv/ep/:id/type"] =
                bgmEpisode.Type == Episode.EpisodeType.SP ? "SP" : "Origin",
                ["https://bgm.tv/ep/:id/sort"] =
                bgmEpisode.Sort?.ToString() ?? ""
            };

            await DbContext.SaveChangesAsync();

            var link = await DbContext.Link.Where(l => l.ItemId == episode.Id && l.Address == address)
                .FirstOrDefaultAsync();
            if (link == null)
            {
                link = new();
                DbContext.Link.Add(link);
            }
            link.ItemId = episode.Id;
            link.Address = address;
            link.MimeType = "text/html;kind=bgm.tv";

            await DbContext.SaveChangesAsync();
            await tx.CommitAsync();
        }

        return new ItemController.ItemDto(item);
    }
}
