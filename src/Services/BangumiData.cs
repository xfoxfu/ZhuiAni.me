using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Me.Xfox.ZhuiAnime.Services;

public class BangumiData : BackgroundService
{
    protected IOptionsMonitor<Option> Options { get; set; }
    protected IServiceScopeFactory Scope { get; set; }
    protected ILogger<BangumiData> Logger { get; set; }

    public BangumiData(IOptionsMonitor<Option> options, IServiceScopeFactory scope, ILogger<BangumiData> logger)
    {
        Options = options;
        Scope = scope;
        Logger = logger;
    }

    protected async Task JsonForEach<T>(StreamReader reader, Func<T, Task> fn, CancellationToken ct)
    {
        string? line;
        while (!ct.IsCancellationRequested && (line = await reader.ReadLineAsync()) != null)
        {
            Logger.LogTrace("Fount item `{}`", line);
            try
            {
                var data = JsonSerializer.Deserialize<T>(line);
                if (data == null)
                {
                    throw new NullReferenceException("data deserialized is null");
                }
                await fn(data);
            }
            catch (JsonException e)
            {
                Logger.LogError(e, "Unable to handle line `{}`", line);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to process line `{}` because of {}", line, e.Message);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken st)
    {
        using var scope = Scope.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ZAContext>();

        // Logger.LogInformation("Reading subjects from {}", Options.CurrentValue.Subject);
        // using var subjectReader = new StreamReader(Options.CurrentValue.Subject);
        // await JsonForEach<BangumiSubject>(subjectReader, async subject =>
        //     {
        //         var link = new Uri($"https://bgm.tv/subject/{subject.Id}");

        //         var anime = await dbContext.Anime.Where(a => a.BangumiLink == link).FirstOrDefaultAsync(st);
        //         if (anime == null) { anime = new(); dbContext.Anime.Add(anime); }

        //         anime.Title = subject.Name;
        //         anime.BangumiLink = link;
        //     }, st);
        // await dbContext.SaveChangesAsync(st);
        // Logger.LogInformation("Subjects synchronized");

        Logger.LogInformation("Reading episodes from {}", Options.CurrentValue.Episodes);
        using var episodeReader = new StreamReader(Options.CurrentValue.Episodes);
        await JsonForEach<BangumiEpisode>(episodeReader, async bgmEp =>
            {
                var link = new Uri($"https://bgm.tv/ep/{bgmEp.Id}");

                var episode = await dbContext.Episode.Where(a => a.BangumiLink == link).FirstOrDefaultAsync(st);
                var anime = await dbContext.Anime
                    .Where(a => a.BangumiLink == new Uri($"https://bgm.tv/subject/{bgmEp.SubjectId}"))
                    .FirstOrDefaultAsync(st);
                if (anime == null) throw new Exception($"Anime {bgmEp.SubjectId} does not exist");
                if (anime.Id == 0) throw new Exception($"Anime {bgmEp.SubjectId} does not exist");
                if (episode == null) { episode = new(); dbContext.Episode.Add(episode); }

                episode.Title = bgmEp.Name;
                episode.Anime = anime;
                episode.AnimeId = anime.Id;
                episode.BangumiLink = link;

            }, st);
        await dbContext.SaveChangesAsync(st);
        Logger.LogInformation("Episodes synchronized");
    }

    public record Option
    {
        public const string LOCATION = "Bangumi:Data";

        public string Character { get; init; } = string.Empty;
        public string Episodes { get; init; } = string.Empty;
        public string PersonCharacters { get; init; } = string.Empty;
        public string Person { get; init; } = string.Empty;
        public string SubjectCharacters { get; init; } = string.Empty;
        public string SubjectPersons { get; init; } = string.Empty;
        public string SubjectRelations { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;

        public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
        {
            builder.Services.Configure<Option>(
                builder.Configuration.GetSection(LOCATION));
            return builder;
        }
    }

    public record BangumiSubject
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// 条目类型：
        /// 1 为 书籍，
        /// 2 为 动画，
        /// 3 为 音乐，
        /// 4 为 游戏，
        /// 6 为 三次元，
        /// 没有 5。
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("name_cn")]
        public string NameCN { get; set; } = string.Empty;

        [JsonPropertyName("infobox")]
        public string InfoBox { get; set; } = string.Empty;

        /// <summary>
        /// TV, Web, 欧美剧, PS4...
        /// </summary>
        [JsonPropertyName("platform")]
        public int Platform { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("nsfw")]
        public int NSFW { get; set; }
    }

    public record BangumiEpisode
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("name_cn")]
        public string NameCn { get; set; } = string.Empty;
        [JsonPropertyName("sort")]
        public double Sort { get; set; }
        [JsonPropertyName("subject_id")]
        public int SubjectId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("airdate")]
        public string Airdate { get; set; } = string.Empty;
        [JsonPropertyName("disc")]
        public int Disc { get; set; }
    }
}
