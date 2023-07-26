using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public record Subject
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("type")]
    public required SubjectType Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("name_cn")]
    public required string NameCn { get; init; }

    [JsonPropertyName("summary")]
    public required string Summary { get; init; }

    [JsonPropertyName("nsfw")]
    public required bool Nsfw { get; init; }

    [JsonPropertyName("locked")]
    public required bool Locked { get; init; }

    [JsonPropertyName("date")]
    public required string Date { get; init; }

    [JsonPropertyName("platform")]
    public required string Platform { get; init; }

    [JsonPropertyName("images")]
    public required ImagesData Images { get; init; }

    [JsonPropertyName("infobox")]
    public required ICollection<Item> Infobox { get; init; }

    [JsonPropertyName("volumes")]
    public required int Volumes { get; init; }

    [JsonPropertyName("eps")]
    public required int Eps { get; init; }

    [JsonPropertyName("total_episodes")]
    public required int TotalEpisodes { get; init; }

    [JsonPropertyName("rating")]
    public required RatingData Rating { get; init; }

    [JsonPropertyName("collection")]
    public required CollectionData Collection { get; init; }

    [JsonPropertyName("tags")]
    public required IEnumerable<TagData> Tags { get; init; }

    public record ImagesData
    {
        [JsonPropertyName("large")]
        public required string Large { get; init; }

        [JsonPropertyName("common")]
        public required string Common { get; init; }

        [JsonPropertyName("medium")]
        public required string Medium { get; init; }

        [JsonPropertyName("small")]
        public required string Small { get; init; }

        [JsonPropertyName("grid")]
        public required string Grid { get; init; }
    }

    public record RatingData
    {
        [JsonPropertyName("rank")]
        public required int Rank { get; init; }

        [JsonPropertyName("total")]
        public required int Total { get; init; }

        [JsonPropertyName("count")]
        public required CountData Count { get; init; }

        [JsonPropertyName("score")]
        public required double Score { get; init; }

        public record CountData
        {
            [JsonPropertyName("1")]
            public required int Rate1Count { get; init; }

            [JsonPropertyName("2")]
            public required int Rate2Count { get; init; }

            [JsonPropertyName("3")]
            public required int Rate3Count { get; init; }

            [JsonPropertyName("4")]
            public required int Rate4Count { get; init; }

            [JsonPropertyName("5")]
            public required int Rate5Count { get; init; }

            [JsonPropertyName("6")]
            public required int Rate6Count { get; init; }

            [JsonPropertyName("7")]
            public required int Rate7Count { get; init; }

            [JsonPropertyName("8")]
            public required int Rate8Count { get; init; }

            [JsonPropertyName("9")]
            public required int Rate9Count { get; init; }

            [JsonPropertyName("10")]
            public required int Rate10Count { get; init; }
        }
    }

    public record CollectionData
    {
        [JsonPropertyName("wish")]
        public required int Wish { get; init; }

        [JsonPropertyName("collect")]
        public required int Collect { get; init; }

        [JsonPropertyName("doing")]
        public required int Doing { get; init; }

        [JsonPropertyName("on_hold")]
        public required int OnHold { get; init; }

        [JsonPropertyName("dropped")]
        public required int Dropped { get; init; }
    }

    public record TagData
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("count")]
        public required int Count { get; init; }
    }
}
