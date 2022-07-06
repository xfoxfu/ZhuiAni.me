using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Me.Xfox.ZhuiAnime.Services;

public class BangumiClient
{
    protected const string BANGUMI_API_HOST = "https://api.bgm.tv/";
    protected BangumiApi BgmApi { get; init; }
    protected HttpClient Client { get; init; }
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
        Client = new(new HttpClientHandler()
        {
            AllowAutoRedirect = false,
        });
        BgmApi = new(BANGUMI_API_HOST, Client);

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
    public enum SubjectImageType
    {
        small,
        grid,
        large,
        medium,
        common,
    }

    public async Task<Models.Anime> SubjectImportToAnimeAsync(int subjectId, CancellationToken ct = default)
    {
        using var scope = Scope.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ZAContext>();

        var bgmAnime = await BgmApi.GetSubjectByIdAsync(subjectId, ct);
        if (bgmAnime.Type != (int)SubjectType._2) throw new Exception($"subject {subjectId} is not anime");

        var link = new Uri($"https://bgm.tv/subject/{subjectId}");
        var anime = await dbContext.Anime.Where(a => a.BangumiLink == link).FirstOrDefaultAsync(ct);
        if (anime == null)
        {
            anime = new();
            dbContext.Anime.Add(anime);
        }

        anime.Title = bgmAnime.Name;
        anime.BangumiLink = link;
        anime.Image = await GetSubjectImageAsync(subjectId, ct: ct);

        await dbContext.SaveChangesAsync(ct);
        return anime;
    }

    /// <summary>
    /// Save subject (anime) image to database.
    /// </summary>
    /// <param name="subjectId">条目 ID</param>
    /// <param name="type">枚举值 {small|grid|large|medium|common}</param>
    /// <exception cref="ArgumentException">subjectId is not found in database.</exception>
    public async Task<byte[]> GetSubjectImageAsync(
        int subjectId,
        SubjectImageType type = SubjectImageType.common,
        CancellationToken ct = default)
    {
        string loc = string.Empty;
        try
        {
            await BgmApi.GetSubjectImageByIdAsync(subjectId, type.ToString(), ct);
        }
        catch (ApiException e)
        {
            if (e.StatusCode != 302) throw e;
            loc = e.Headers.GetValueOrDefault("Location")?.FirstOrDefault()
                ?? e.Headers.GetValueOrDefault("location")?.FirstOrDefault()
                ?? string.Empty;
        }
        if (loc == string.Empty) throw new Exception($"no image for {subjectId} present on bgm.tv");
        var data = await Client.GetByteArrayAsync(loc, ct);
        return data;
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
