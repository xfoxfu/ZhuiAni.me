using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record PaginatedResult<T>(
    [property:JsonPropertyName("total")]
    uint Total,
    [property:JsonPropertyName("limit")]
    uint Limit,
    [property:JsonPropertyName("offset")]
    uint Offset,
    [property:JsonPropertyName("data")]
    IEnumerable<T> Data
);
