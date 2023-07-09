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

  [JsonConverter(typeof(ContentArrayConverter))]
  [JsonPropertyName("content")]
  public IEnumerable<IEnumerable<string>> Content { get; set; } = new List<IEnumerable<string>>();

  [JsonPropertyName("size")]
  public string? Size { get; set; } = string.Empty;

  [JsonPropertyName("btskey")]
  public string? Btskey { get; set; } = string.Empty;

  [JsonPropertyName("sync")]
  public IDictionary<string, JsonElement>? Sync { get; set; } = new Dictionary<string, JsonElement>();

  internal class ContentArrayConverter : JsonConverter<IEnumerable<IEnumerable<string>>>
  {
    public override IEnumerable<IEnumerable<string>>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartArray)
      {
        throw new JsonException();
      }

      var list = new List<IEnumerable<string>>();
      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndArray)
        {
          return list;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
          list.Add(new List<string> { reader.GetString() ?? string.Empty });
        }
        else if (reader.TokenType != JsonTokenType.StartArray)
        {
          throw new JsonException();
        }
        else
        {
          var subList = new List<string>();
          while (reader.Read())
          {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
              list.Add(subList);
              break;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
              throw new JsonException();
            }

            subList.Add(reader.GetString() ?? string.Empty);
          }
        }
      }

      throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<IEnumerable<string>> value, JsonSerializerOptions options)
    {
      writer.WriteStartArray();
      foreach (var item in value)
      {
        writer.WriteStartArray();
        foreach (var subItem in item)
        {
          writer.WriteStringValue(subItem);
        }
        writer.WriteEndArray();
      }
      writer.WriteEndArray();
    }
  }
}
