using System.Text.Json;
using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Models;

public record Item(
    [property: JsonPropertyName("key")]
    string Key,

    [property:JsonPropertyName("value")]
    [property:JsonConverter(typeof(Item.ValueJsonConverter))]
    Item.ItemValue Value
)
{
    public record ItemValue();

    public record StringItemValue(string Value) : ItemValue;

    public record KVListItemValue(ICollection<KVItem> Value) : ItemValue;

    public record KVItem : ItemValue
    {
        [JsonPropertyName("k")]
        public string? Key { get; init; }
        [JsonPropertyName("v")]
        public string Value { get; init; }

        [JsonConstructor]
        public KVItem(string? Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        public KVItem(string Value) : this(null, Value) { }
    }

    public class ValueJsonConverter : JsonConverter<ItemValue>
    {
        public override ItemValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var obj = JsonSerializer.Deserialize<JsonElement>(ref reader);
            if (obj.ValueKind == JsonValueKind.String)
            {
                return new StringItemValue(obj.GetString()!);
            }
            var list = obj.Deserialize<ICollection<KVItem>>()!;
            return new KVListItemValue(list);
        }

        public override void Write(Utf8JsonWriter writer, ItemValue value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
