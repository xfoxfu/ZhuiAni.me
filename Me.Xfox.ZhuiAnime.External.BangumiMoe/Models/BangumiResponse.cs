using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.BangumiMoe.Models;

public class BangumiResponse
{
  [JsonPropertyName("torrents")]
  public IEnumerable<BangumiTorrent> Torrents { get; set; } = new List<BangumiTorrent>();

  [JsonPropertyName("count")]
  public ulong? Count { get; set; }

  [JsonPropertyName("page_count")]
  public ulong PageCount { get; set; }
}
