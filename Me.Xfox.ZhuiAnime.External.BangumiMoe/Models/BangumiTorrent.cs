using System.Text.Json;
using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.BangumiMoe.Models;

public class BangumiTorrent
{
  [JsonPropertyName("_id")]
  public string Id { get; set; } = string.Empty;

  [JsonPropertyName("category_tag_id")]
  public string CategoryTagId { get; set; } = string.Empty;

  [JsonPropertyName("title")]
  public string Title { get; set; } = string.Empty;

  [JsonPropertyName("introduction")]
  public string Introduction { get; set; } = string.Empty;

  [JsonPropertyName("tag_ids")]
  public IEnumerable<string> TagIds { get; set; } = new List<string>();

  [JsonPropertyName("comments")]
  public ulong? Comments { get; set; }

  [JsonPropertyName("downloads")]
  public ulong Downloads { get; set; }

  [JsonPropertyName("finished")]
  public ulong Finished { get; set; }

  [JsonPropertyName("leechers")]
  public ulong Leechers { get; set; }

  [JsonPropertyName("seeders")]
  public ulong Seeders { get; set; }

  [JsonPropertyName("uploader_id")]
  public string UploaderId { get; set; } = string.Empty;

  [JsonPropertyName("team_id")]
  public string TeamId { get; set; } = string.Empty;

  [JsonPropertyName("publish_time")]
  public string PublishTime { get; set; } = string.Empty;

  [JsonPropertyName("magnet")]
  public string Magnet { get; set; } = string.Empty;

  [JsonPropertyName("infoHash")]
  public string InfoHash { get; set; } = string.Empty;

  [JsonPropertyName("file_id")]
  public string FileId { get; set; } = string.Empty;

  [JsonPropertyName("teamsync")]
  public bool? Teamsync { get; set; }

  [JsonPropertyName("content")]
  public IEnumerable<IEnumerable<string>> Content { get; set; } = new List<IEnumerable<string>>();

  [JsonPropertyName("size")]
  public string? Size { get; set; } = string.Empty;

  [JsonPropertyName("btskey")]
  public string? Btskey { get; set; } = string.Empty;

  [JsonPropertyName("sync")]
  public IDictionary<string, string>? Sync { get; set; } = new Dictionary<string, string>();
}
