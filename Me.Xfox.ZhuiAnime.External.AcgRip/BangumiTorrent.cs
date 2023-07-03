using System.Text.Json;
using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.AcgRip;

public class BangumiTorrent
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
