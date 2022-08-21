using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.External.Bangumi;
using Me.Xfox.ZhuiAnime.External.Bangumi.Models;
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
        BgmApi = new(BANGUMI_API_HOST, Options.CurrentValue.UserAgent);

        Options.OnChange(opt =>
        {
            BgmApi.UserAgent = opt.UserAgent;
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
        try
        {
            using var scope = Scope.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ZAContext>();

            var bgmAnime = await BgmApi.GetSubjectAsync(subjectId, ct);
            if (bgmAnime.Type != SubjectType.Anime) throw new Exception($"subject {subjectId} is not anime");

            var link = new Uri($"https://bgm.tv/subject/{subjectId}");
            var anime = await dbContext.Anime.Where(a => a.BangumiLink == link).FirstOrDefaultAsync(ct);
            if (anime == null)
            {
                anime = new();
                dbContext.Anime.Add(anime);
            }

            anime.Title = bgmAnime.Name;
            anime.BangumiLink = link;
            anime.Image = await BgmApi.GetBytesAsync(bgmAnime.Images.Common, ct);

            await dbContext.SaveChangesAsync(ct);
            return anime;
        }
        catch (Exception e)
        {
            throw new BangumiException();
        }
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
        public BangumiException(Exception cause)
        {
        }
    }
}
