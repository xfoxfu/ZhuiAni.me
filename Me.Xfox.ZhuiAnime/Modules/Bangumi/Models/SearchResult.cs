using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public record SearchResult<T>
{
    [JsonPropertyName("results")]
    public required uint Count { get; init; }

    [JsonPropertyName("list")]
    public required IEnumerable<T> Items { get; init; }
};
