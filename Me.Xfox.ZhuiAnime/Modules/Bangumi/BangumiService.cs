using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using AppModels = Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

public class BangumiService
{
    public IServiceProvider Services { get; init; }
    public Client.BangumiApi BgmApi { get; init; }

    public BangumiService(IServiceProvider services, Client.BangumiApi bgmApi)
    {
        Services = services;
        BgmApi = bgmApi;
    }

    public async Task<uint> ImportSubjectGetId(int id)
    {
        var item = await ImportSubject(id);
        return item.Id;
    }

    public async Task<AppModels.Item> ImportSubject(int id)
    {
        using var scope = Services.CreateScope();
        using var DbContext = scope.ServiceProvider.GetRequiredService<ZAContext>();

        AppModels.Category category;
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

        AppModels.Item item;
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

        return item;
    }
}
