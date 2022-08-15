using System.Text.Json;
using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record Item
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    [JsonConverter(typeof(ValueJsonConverter))]
    public ItemValue Value { get; set; } = default!;

    public record ItemValue { }
    public record StringItemValue : ItemValue
    {
        public string Value { get; set; } = string.Empty;
    }
    public record KVListItemValue : ItemValue
    {
        public ICollection<KVItem> Value { get; set; } = default!;
    }
    public record KVItem : ItemValue
    {
        [JsonPropertyName("k")]
        public string? Key { get; set; }
        [JsonPropertyName("v")]
        public string Value { get; set; } = string.Empty;
    }

    public class ValueJsonConverter : JsonConverter<ItemValue>
    {
        public override ItemValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var obj = JsonSerializer.Deserialize<JsonElement>(ref reader);
            if (obj.ValueKind == JsonValueKind.String)
            {
                return new StringItemValue { Value = obj.GetString()! };
            }
            Console.WriteLine("{0}", obj);
            var list = obj.Deserialize<ICollection<KVItem>>()!;
            return new KVListItemValue { Value = list };
        }

        public override void Write(Utf8JsonWriter writer, ItemValue value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
