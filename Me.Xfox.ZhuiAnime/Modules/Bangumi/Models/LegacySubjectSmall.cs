using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public class LegacySubjectSmall
{
    /// <summary>
    /// 放送开始日期
    /// </summary>
    [JsonPropertyName("air_date")]
    public required string AirDate { get; set; }

    /// <summary>
    /// 放送星期
    /// </summary>
    [JsonPropertyName("air_weekday")]
    public required long? AirWeekday { get; set; }

    /// <summary>
    /// 收藏人数
    /// </summary>
    [JsonPropertyName("collection")]
    public required CollectionData Collection { get; set; }

    /// <summary>
    /// 话数
    /// </summary>
    [JsonPropertyName("eps")]
    public required long? Eps { get; set; }

    /// <summary>
    /// 话数
    /// </summary>
    [JsonPropertyName("eps_count")]
    public required long? EpsCount { get; set; }

    /// <summary>
    /// 条目 ID
    /// </summary>
    [JsonPropertyName("id")]
    public required long? Id { get; set; }

    /// <summary>
    /// 封面
    /// </summary>
    [JsonPropertyName("images")]
    public required ImagesData Images { get; set; }

    /// <summary>
    /// 条目名称
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// 条目中文名称
    /// </summary>
    [JsonPropertyName("name_cn")]
    public required string NameCn { get; set; }

    /// <summary>
    /// 排名
    /// </summary>
    [JsonPropertyName("rank")]
    public required long? Rank { get; set; }

    /// <summary>
    /// 评分
    /// </summary>
    [JsonPropertyName("rating")]
    public required RatingData Rating { get; set; }

    /// <summary>
    /// 剧情简介
    /// </summary>
    [JsonPropertyName("summary")]
    public required string Summary { get; set; }

    /// <summary>
    /// 条目类型
    /// - `1` 为 书籍
    /// - `2` 为 动画
    /// - `3` 为 音乐
    /// - `4` 为 游戏
    /// - `6` 为 三次元
    ///
    /// 没有 `5`
    /// </summary>
    [JsonPropertyName("type")]
    public required long? Type { get; set; }

    /// <summary>
    /// 条目地址
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    /// <summary>
    /// 收藏人数
    /// </summary>
    public class CollectionData
    {
        /// <summary>
        /// 做过
        /// </summary>
        [JsonPropertyName("collect")]
        public required long? Collect { get; set; }

        /// <summary>
        /// 在做
        /// </summary>
        [JsonPropertyName("doing")]
        public required long? Doing { get; set; }

        /// <summary>
        /// 抛弃
        /// </summary>
        [JsonPropertyName("dropped")]
        public required long? Dropped { get; set; }

        /// <summary>
        /// 搁置
        /// </summary>
        [JsonPropertyName("on_hold")]
        public required long? OnHold { get; set; }

        /// <summary>
        /// 想做
        /// </summary>
        [JsonPropertyName("wish")]
        public required long? Wish { get; set; }
    }

    /// <summary>
    /// 封面
    /// </summary>
    public class ImagesData
    {
        [JsonPropertyName("common")]
        public required string Common { get; set; }

        [JsonPropertyName("grid")]
        public required string Grid { get; set; }

        [JsonPropertyName("large")]
        public required string Large { get; set; }

        [JsonPropertyName("medium")]
        public required string Medium { get; set; }

        [JsonPropertyName("small")]
        public required string Small { get; set; }
    }

    /// <summary>
    /// 评分
    /// </summary>
    public class RatingData
    {
        /// <summary>
        /// 各分值评分人数
        /// </summary>
        [JsonPropertyName("count")]
        public required Count Count { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [JsonPropertyName("score")]
        public required double? Score { get; set; }

        /// <summary>
        /// 总评分人数
        /// </summary>
        [JsonPropertyName("total")]
        public required long? Total { get; set; }
    }

    /// <summary>
    /// 各分值评分人数
    /// </summary>
    public class Count
    {
        [JsonPropertyName("1")]
        public required long? The1 { get; set; }

        [JsonPropertyName("10")]
        public required long? The10 { get; set; }

        [JsonPropertyName("2")]
        public required long? The2 { get; set; }

        [JsonPropertyName("3")]
        public required long? The3 { get; set; }

        [JsonPropertyName("4")]
        public required long? The4 { get; set; }

        [JsonPropertyName("5")]
        public required long? The5 { get; set; }

        [JsonPropertyName("6")]
        public required long? The6 { get; set; }

        [JsonPropertyName("7")]
        public required long? The7 { get; set; }

        [JsonPropertyName("8")]
        public required long? The8 { get; set; }

        [JsonPropertyName("9")]
        public required long? The9 { get; set; }
    }
}
