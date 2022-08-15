using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record Subject
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public SubjectType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("name_cn")]
    public string NameCn { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    /// <summary>
    /// air date in `YYYY-MM-DD` format
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// TV, Web, 欧美剧, PS4...
    /// </summary>
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public ImagesData Images { get; set; } = default!;

    [JsonPropertyName("infobox")]
    public ICollection<Item> Infobox { get; set; } = default!;

    /// <summary>
    /// 书籍条目的册数，由旧服务端从wiki中解析
    /// </summary>
    [JsonPropertyName("volumes")]
    public int Volumes { get; set; }

    /// <summary>
    /// 由旧服务端从wiki中解析，对于书籍条目为`话数`
    /// </summary>
    [JsonPropertyName("eps")]
    public int Eps { get; set; }

    /// <summary>
    /// 数据库中的章节数量
    /// </summary>
    [JsonPropertyName("total_episodes")]
    public int TotalEpisodes { get; set; }

    [JsonPropertyName("rating")]
    public RatingData Rating { get; set; } = new RatingData();

    [JsonPropertyName("collection")]
    public CollectionData Collection { get; set; } = new CollectionData();

    [JsonPropertyName("tags")]
    public IEnumerable<TagData> Tags { get; set; } = new List<TagData>();

    public record ImagesData
    {
        [JsonPropertyName("large")] public string Large { get; set; } = string.Empty;
        [JsonPropertyName("common")] public string Common { get; set; } = string.Empty;
        [JsonPropertyName("medium")] public string Medium { get; set; } = string.Empty;
        [JsonPropertyName("small")] public string Small { get; set; } = string.Empty;
        [JsonPropertyName("grid")] public string Grid { get; set; } = string.Empty;
    }

    public record RatingData
    {
        [JsonPropertyName("rank")] public int Rank { get; set; }
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("count")] public CountData Count { get; set; } = default!;
        [JsonPropertyName("score")] public double Score { get; set; }

        public record CountData
        {
            [JsonPropertyName("1")] public int Rate1Count { get; set; }
            [JsonPropertyName("2")] public int Rate2Count { get; set; }
            [JsonPropertyName("3")] public int Rate3Count { get; set; }
            [JsonPropertyName("4")] public int Rate4Count { get; set; }
            [JsonPropertyName("5")] public int Rate5Count { get; set; }
            [JsonPropertyName("6")] public int Rate6Count { get; set; }
            [JsonPropertyName("7")] public int Rate7Count { get; set; }
            [JsonPropertyName("8")] public int Rate8Count { get; set; }
            [JsonPropertyName("9")] public int Rate9Count { get; set; }
            [JsonPropertyName("10")] public int Rate10Count { get; set; }
        }
    }

    public record CollectionData
    {
        [JsonPropertyName("wish")] public int Wish { get; set; }
        [JsonPropertyName("collect")] public int Collect { get; set; }
        [JsonPropertyName("doing")] public int Doing { get; set; }
        [JsonPropertyName("on_hold")] public int OnHold { get; set; }
        [JsonPropertyName("dropped")] public int Dropped { get; set; }
    }

    public record TagData
    {
        [JsonPropertyName("name")] public string Name { get; set; } = default!;
        [JsonPropertyName("count")] public int Count { get; set; }
    }
}
