using System.Text.Json;
using System.Text.Json.Serialization;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;

namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory.Sources;

public class AcgRipSource : ISource
{
    public string Name => "acg.rip";

    public string Url => "https://acg.rip";

    public async Task<IList<Torrent>> GetPageAsync(uint id)
    {
        var feed = await FeedReader.ReadAsync(
          $"https://acg.rip/page/{id}.xml",
          userAgent: "xfoxfu/ZhuiAni.me (https://github.com/xfoxfu/ZhuiAni.me)");
        return feed.Items.Select(ConvertFeedItem).Select(ConvertToTorrent).ToList();
    }

    protected AcgRipItem ConvertFeedItem(FeedItem item)
    {
        return new AcgRipItem
        {
            Title = item.Title,
            Description = item.Description,
            PubDate = item.PublishingDateString,
            Link = item.Link,
            Guid = item.Id,
            Enclosure = (item.SpecificItem as Rss20FeedItem)?.Enclosure.Url,
        };
    }

    protected Torrent ConvertToTorrent(AcgRipItem item)
    {
        return new Torrent
        {
            OriginSite = Url,
            OriginId = item.Guid.Replace("https://acg.rip/t/", ""),
            Title = item.Title,
            Size = null,
            Contents = null,
            PublishedAt = DateTimeOffset.Parse(item.PubDate).ToUniversalTime(),
            OriginData = JsonSerializer.SerializeToDocument(item),
            LinkTorrent = item.Enclosure,
            LinkMagnet = null,
        };
    }

    public class AcgRipItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("pub_date")]
        public string PubDate { get; set; } = string.Empty;

        [JsonPropertyName("link")]
        public string Link { get; set; } = string.Empty;

        [JsonPropertyName("guid")]
        public string Guid { get; set; } = string.Empty;

        [JsonPropertyName("enclosure")]
        public string? Enclosure { get; set; } = string.Empty;
    }
}
