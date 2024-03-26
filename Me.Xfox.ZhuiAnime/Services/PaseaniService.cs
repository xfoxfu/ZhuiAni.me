using System.Text.Json.Serialization;
using Flurl.Http;

namespace Me.Xfox.ZhuiAnime.Services;

public class PaseaniService
{
    protected ILogger<PaseaniService> Logger { get; init; }

    public PaseaniService(ILogger<PaseaniService> logger)
    {
        Logger = logger;
    }

    public const string API_ENDPOINT = "https://paseani.zhuiani.me/info";

    public record Tag
    {
        [JsonPropertyName("type")] public required string Type;
        [JsonPropertyName("value")] public required string Value;
        [JsonPropertyName("parser")] public required string Parser;
    }

    public record Error
    {
        [JsonPropertyName("message")] public required string Message;
        [JsonPropertyName("parser")] public required string Parser;
    }

    public record ParseResult
    {
        [JsonPropertyName("tags")] public required IEnumerable<Tag> Tags;
        [JsonPropertyName("errors")] public required IEnumerable<Error> Errors;
    }

    public async Task<ParseResult> Parse(string title)
    {
        return await new Flurl.Url(API_ENDPOINT)
            .SetQueryParam("name", title)
            .GetJsonAsync<ParseResult>();
    }

    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<PaseaniService>();
        return builder;
    }
}
