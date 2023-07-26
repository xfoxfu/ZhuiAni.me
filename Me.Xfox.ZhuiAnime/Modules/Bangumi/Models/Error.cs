using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public record Error
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    // TODO: error detail is ignored.
}
