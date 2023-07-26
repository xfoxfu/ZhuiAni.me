using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public record Episode
{
    [JsonPropertyName("id")]
    public required uint Id { get; init; }

    [JsonPropertyName("type")]
    public required EpisodeType Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("name_cn")]
    public required string NameCn { get; init; }

    [JsonPropertyName("sort")]
    public double? Sort { get; init; }

    [JsonPropertyName("ep")]
    public double? Ep { get; init; }

    [JsonPropertyName("airdate")]
    public required string AirDate { get; init; }

    [JsonPropertyName("comment")]
    public required int CommentCount { get; init; }

    [JsonPropertyName("duration")]
    public required string Duration { get; init; }

    [JsonPropertyName("desc")]
    public required string Description { get; init; }

    [JsonPropertyName("disc")]
    public required int DiscCount { get; init; }

    [JsonPropertyName("duration_seconds")]
    public required int DurationSeconds { get; init; }


    public enum EpisodeType
    {
        Origin = 0,
        SP = 1,
        OP = 2,
        ED = 3,
    }
}
