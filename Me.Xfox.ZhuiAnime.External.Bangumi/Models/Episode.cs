using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record Episode(
    [property:JsonPropertyName("id")]
    uint Id,

    [property:JsonPropertyName("type")]
    Episode.EpisodeType Type,

    [property:JsonPropertyName("name")]
    string Name,

    [property:JsonPropertyName("name_cn")]
    string NameCn,

    [property:JsonPropertyName("sort")]
    double? Sort,

    [property:JsonPropertyName("ep")]
    double? Ep,
    /// <summary>
    /// air date in `YYYY-MM-DD` format
    /// </summary>

    [property:JsonPropertyName("airdate")]
    string AirDate,

    [property:JsonPropertyName("comment")]
    int CommentCount,

    [property:JsonPropertyName("duration")]
    string Duration,

    [property:JsonPropertyName("desc")]
    string Description,

    [property:JsonPropertyName("disc")]
    int DiscCount,

    [property:JsonPropertyName("duration_seconds")]
    int DurationSeconds
)
{
    public enum EpisodeType
    {
        Origin = 0,
        SP = 1,
        OP = 2,
        ED = 3,
    }
}
