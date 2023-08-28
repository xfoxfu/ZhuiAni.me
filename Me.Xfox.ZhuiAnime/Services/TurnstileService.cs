using Microsoft.Extensions.Options;
using Flurl;
using Flurl.Http;
using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Services;

public class TurnstileService
{
    protected ILogger<TurnstileService> Logger { get; init; }
    protected IOptionsMonitor<Option> Options { get; set; }
    protected bool Enabled => Options.CurrentValue.Enabled;
    protected string Secret => Options.CurrentValue.Secret;

    public TurnstileService(ILogger<TurnstileService> logger, IOptionsMonitor<Option> options)
    {
        Logger = logger;
        Options = options;
    }

    public record TurnstileResponse
    {
        [JsonPropertyName("success")]
        public required bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; } = string.Empty;

        [JsonPropertyName("error-codes")]
        public IEnumerable<string> ErrorCodes { get; set; } = new List<string>();

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("cdata")]
        public string CData { get; set; } = string.Empty;
    }

    public async Task Validate(string response)
    {
        if (!Enabled) return;
        var result = await "https://challenges.cloudflare.com/turnstile/v0/siteverify"
            .PostJsonAsync(new { secret = Secret, response })
            .ReceiveJson<TurnstileResponse>();
        if (!result.Success)
        {
            throw new ZhuiAnimeError.CaptchaInvalid(response, result.ErrorCodes);
        }
    }

    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.Configure<Option>(builder.Configuration.GetSection(Option.LOCATION));
        builder.Services.AddSingleton<TurnstileService>();
        return builder;
    }

    public class Option
    {
        public const string LOCATION = "Captcha";

        public bool Enabled { get; set; }

        public string Secret { get; set; } = string.Empty;
    }
}
