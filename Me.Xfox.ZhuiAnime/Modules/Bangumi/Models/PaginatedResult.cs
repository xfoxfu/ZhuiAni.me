using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public record PaginatedResult<T>
{
    [JsonPropertyName("total")]
    public required uint Total { get; init; }

    [JsonPropertyName("limit")]
    public required uint Limit { get; init; }

    [JsonPropertyName("offset")]
    public required uint Offset { get; init; }

    [JsonPropertyName("data")]
    public required IEnumerable<T> Data { get; init; }
};
