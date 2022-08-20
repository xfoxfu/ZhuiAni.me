using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record Subject
(
    [property:JsonPropertyName("id")]
    int Id,

    [property:JsonPropertyName("type")]
    SubjectType Type,

    [property:JsonPropertyName("name")]
    string Name,

    [property:JsonPropertyName("name_cn")]
    string NameCn,

    [property:JsonPropertyName("summary")]
    string Summary,

    [property:JsonPropertyName("nsfw")]
    bool Nsfw,

    [property:JsonPropertyName("locked")]
    bool Locked,

    /// <summary>
    /// air date in `YYYY-MM-DD` format
    /// </summary>
    [property:JsonPropertyName("date")]
    string Date,

    /// <summary>
    /// TV, Web, 欧美剧, PS4...
    /// </summary>
    [property:JsonPropertyName("platform")]
    string Platform,

    [property:JsonPropertyName("images")]
    Subject.ImagesData Images,

    [property:JsonPropertyName("infobox")]
     ICollection<Item> Infobox,

    /// <summary>
    /// 书籍条目的册数，由旧服务端从wiki中解析
    /// </summary>
    [property:JsonPropertyName("volumes")]
    int Volumes,

    /// <summary>
    /// 由旧服务端从wiki中解析，对于书籍条目为`话数`
    /// </summary>
    [property:JsonPropertyName("eps")]
    int Eps,

    /// <summary>
    /// 数据库中的章节数量
    /// </summary>
    [property:JsonPropertyName("total_episodes")]
    int TotalEpisodes,

    [property:JsonPropertyName("rating")]
    Subject.RatingData Rating,

    [property:JsonPropertyName("collection")]
    Subject.CollectionData Collection,

    [property:JsonPropertyName("tags")]
    IEnumerable<Subject.TagData> Tags
)
{
    public record ImagesData
    (
        [property: JsonPropertyName("large")] string Large,
        [property: JsonPropertyName("common")] string Common,
        [property: JsonPropertyName("medium")] string Medium,
        [property: JsonPropertyName("small")] string Small,
        [property: JsonPropertyName("grid")] string Grid
    );

    public record RatingData
    (
        [property: JsonPropertyName("rank")] int Rank,
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("count")] RatingData.CountData Count,
        [property: JsonPropertyName("score")] double Score
    )
    {

        public record CountData
        (
            [property: JsonPropertyName("1")]
            int Rate1Count,
            [property: JsonPropertyName("2")] int Rate2Count,
            [property: JsonPropertyName("3")] int Rate3Count,
            [property: JsonPropertyName("4")] int Rate4Count,
            [property: JsonPropertyName("5")] int Rate5Count,
            [property: JsonPropertyName("6")] int Rate6Count,
            [property: JsonPropertyName("7")] int Rate7Count,
            [property: JsonPropertyName("8")] int Rate8Count,
            [property: JsonPropertyName("9")] int Rate9Count,
            [property: JsonPropertyName("10")] int Rate10Count
        );
    }

    public record CollectionData
    (
        [property: JsonPropertyName("wish")] int Wish,
        [property: JsonPropertyName("collect")] int Collect,
        [property: JsonPropertyName("doing")] int Doing,
        [property: JsonPropertyName("on_hold")] int OnHold,
        [property: JsonPropertyName("dropped")] int Dropped
    );

    public record TagData
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("count")] int Count
    );
}
