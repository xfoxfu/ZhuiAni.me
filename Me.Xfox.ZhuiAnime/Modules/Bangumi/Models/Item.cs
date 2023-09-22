using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public record Item
{
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    [JsonPropertyName("value")]
    [JsonConverter(typeof(ValueJsonConverter))]
    public required ItemValue Value { get; init; }

    public Item() { }

    [SetsRequiredMembers]
    public Item(string Key, ItemValue Value)
    {
        this.Key = Key;
        this.Value = Value;
    }

    [SetsRequiredMembers]
    public Item(string Key, string Value)
    {
        this.Key = Key;
        this.Value = new StringItemValue(Value);
    }

    [SetsRequiredMembers]
    public Item(string Key, ICollection<KVItem> Value)
    {
        this.Key = Key;
        this.Value = new KVListItemValue(Value);
    }

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
            if (reader.TokenType == JsonTokenType.String)
            {
                return new StringItemValue(reader.GetString()!);
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = JsonSerializer.Deserialize<ICollection<KVItem>>(ref reader, options);
                return new KVListItemValue(list!);
            }
            else
            {
                throw new JsonException($"ItemValue should be string or array, found {reader.TokenType} instead.");
            }
        }

        public override void Write(Utf8JsonWriter writer, ItemValue value, JsonSerializerOptions options)
        {
            if (value is StringItemValue s)
            {
                writer.WriteStringValue(s.Value);
            }
            else if (value is KVListItemValue l)
            {
                writer.WriteStartArray();
                foreach (var item in l.Value)
                {
                    writer.WriteStartObject();
                    writer.WriteString("k", item.Key);
                    writer.WriteString("v", item.Value);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
    }
}
