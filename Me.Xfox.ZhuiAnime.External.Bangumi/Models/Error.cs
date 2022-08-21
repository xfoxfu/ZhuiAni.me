using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record Error(
    [property:JsonPropertyName("title")]
    string Title,

    [property:JsonPropertyName("description")]
    string Description

// TODO: error detail is ignored.
);
