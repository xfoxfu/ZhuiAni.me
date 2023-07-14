using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

public interface ISource
{
    string Name { get; }

    string Url { get; }

    Task<IList<Torrent>> GetPageAsync(uint id);

    public async Task<bool> GetPage(uint page, ZAContext db)
    {
        var response = await GetPageAsync(page);

        bool hasNonExistent = false;

        foreach (var item in response)
        {
            if (!await db.Torrent.Where(e => e.OriginSite == item.OriginSite && e.OriginId == item.OriginId).AnyAsync())
            {
                hasNonExistent = true;
                db.Torrent.Add(item);
            }
        }
        await db.SaveChangesAsync();

        return hasNonExistent;
    }
}
