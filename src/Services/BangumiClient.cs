using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Me.Xfox.ZhuiAnime.Services;

public class BangumiClient
{
    protected readonly HttpClient Client = new();
    protected IOptionsMonitor<Option> Options { get; set; }
    protected ILogger<BangumiClient> Logger { get; set; }
    protected IServiceScopeFactory Scope { get; set; }
    protected string AppId { get => Options.CurrentValue.AppId; }
    protected string AppSecret { get => Options.CurrentValue.AppSecret; }

    public BangumiClient(IOptionsMonitor<Option> options, ILogger<BangumiClient> logger, IServiceScopeFactory scope)
    {
        Options = options;
        Logger = logger;
        Scope = scope;

        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Client.DefaultRequestHeaders.Add("User-Agent", Options.CurrentValue.UserAgent);

        Options.OnChange(opt =>
        {
            Client.DefaultRequestHeaders.UserAgent.Clear();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(opt.UserAgent);
        });
    }

    #region Subject
    /// <summary>
    /// Save "common" resolution image to database.
    /// </summary>
    /// <param name="subjectId">条目 ID</param>
    /// <param name="type">枚举值 {small|grid|large|medium|common}</param>
    /// <exception cref="ArgumentException">subjectId is not found in database.</exception>
    public async Task SubjectImageSaveToDatabase(int subjectId, CancellationToken? ct)
    {
        var url = $"https://api.bgm.tv/v0/subjects/{subjectId}/image?type=common";
        var data = await Client.GetByteArrayAsync(url, ct ?? CancellationToken.None);

        using var scope = Scope.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ZAContext>();

        var anime = await dbContext.Anime.FindAsync(new object[] { subjectId }, ct ?? CancellationToken.None);
        if (anime == null)
        {
            throw new ArgumentException($"subjectId {subjectId} not found in database");
        }

        anime.Image = data;
        await dbContext.SaveChangesAsync();
    }
    #endregion

    public record Option
    {
        public const string LOCATION = "Bangumi:Client";

        public string UserAgent { get; init; } = string.Empty;
        public string AppId { get; init; } = string.Empty;
        public string AppSecret { get; init; } = string.Empty;

        public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
        {
            builder.Services.Configure<Option>(
                builder.Configuration.GetSection(LOCATION));
            return builder;
        }
    }

    public class BangumiException : Exception
    {

    }
}
