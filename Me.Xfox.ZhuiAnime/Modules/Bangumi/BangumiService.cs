using System.Collections;
using System.Text.Json;
using Elsa.Expressions.Models;
using Elsa.Extensions;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Memory;
using Me.Xfox.ZhuiAnime.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;
using Microsoft.AspNetCore.Mvc;
using AppModels = Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

public class BangumiService
{
    protected ZAContext DbContext { get; init; }
    protected Client.BangumiApi BgmApi { get; init; }
    protected readonly IWorkflowRunner _workflowHost;

    public BangumiService(Client.BangumiApi bgmApi, ZAContext dbContext, IWorkflowRunner workflowHost)
    {
        BgmApi = bgmApi;
        DbContext = dbContext;
        _workflowHost = workflowHost;
    }

    public async Task<Ulid> ImportSubjectGetId(int id)
    {
        var item = await ImportSubject(id);
        return item.Id;
    }

    public async Task<Subject> GetSubject(int id)
    {
        return await BgmApi.GetSubjectAsync(id);
    }

    public async Task<IEnumerable<Episode>> GetEpisodes(int id)
    {
        return await BgmApi.GetEpisodesAsync(id).ToListAsync();
    }

    public async Task<AppModels.Item> ImportSubject(int id)
    {
        var vars = new
        {
            Subject = new Variable<Subject>(),
            Episodes = new Variable<IEnumerable<Episode>>(),
            AnimeCategory = new Variable<Category>(),
            AnimeItem = new Variable<AppModels.Item>(),
            CurrentEpisode = new Variable<Episode>(),
        };
        var workflow = new Workflow
        {
            Variables = new Variable[] { vars.Subject, vars.Episodes, vars.AnimeCategory, vars.AnimeItem },
            ResultVariable = vars.AnimeItem,
            Root = new Sequence
            {
                Activities =
            {
                new CreateCategory()
                {
                    InTitle = new("アニメ"),
                    Result = new(vars.AnimeCategory),
                },
                new GetBangumiSubject()
                {
                    InSubjectId = new(ctx => ctx.GetInput<int>("SubjectId")),
                    OutSubject = new(vars.Subject),
                    OutEpisodes = new (vars.Episodes),
                },
                new CreateItemWithLink()
                {
                    InTitle = new(ctx => vars.Subject.Get(ctx)?.Name ?? throw new Exception()),
                    InCategoryId = new(ctx => vars.AnimeCategory.Get(ctx)?.Id ?? default),
                    InParentItemId = null,
                    InAnnotations = null,
                    InImageUrl = new(ctx => vars.Subject.Get(ctx)?.Images.Large ?? string.Empty),
                    InLinkAddress = new(ctx => new Uri($"https://bgm.tv/subject/{vars.Subject.Get(ctx)?.Id}")),
                    InLinkMimeType = new("text/html"),
                    OutItem = new(vars.AnimeItem),
                },
                new ForEach()
                {
                    Items = new(vars.Episodes),
                    CurrentValue = new(vars.CurrentEpisode),
                    Body = new Sequence
                    {
                        Activities =
                        {
                            new CreateItemWithLink()
                            {
                                InTitle = new(ctx =>
                                {
                                    var bgmAnime = vars.Subject.Get(ctx) ?? throw new InvalidOperationException();
                                    var bgmEpisode = vars.CurrentEpisode.Get(ctx) ?? throw new InvalidOperationException();
                                    int episodeNameLength = Convert.ToInt32(Math.Ceiling(Math.Log10(bgmAnime.TotalEpisodes + 1)));
                                    string episodeNameFormat = $"{new('0', episodeNameLength)}.###";

                                    var address = new Uri($"https://bgm.tv/ep/{bgmEpisode.Id}");
                                    var prefix = (bgmEpisode.Sort ?? 0).ToString(episodeNameFormat);
                                    if (bgmEpisode.Type == Episode.EpisodeType.SP) prefix = $"SP{prefix}";
                                    return string.IsNullOrEmpty(bgmEpisode.Name) ? prefix : $"{prefix} - {bgmEpisode.Name}";
                                }),
                                InCategoryId = new(ctx => vars.AnimeCategory.Get(ctx)?.Id ?? default),
                                InParentItemId = new(ctx => vars.AnimeItem.Get(ctx)?.Id ?? null),
                                InAnnotations = new(ctx => new Dictionary<string, string> ()
                                {
                                    ["https://bgm.tv/ep/:id/type"] =
                                        vars.CurrentEpisode.Get(ctx)?.Type == Episode.EpisodeType.SP ? "SP" : "Origin",
                                    ["https://bgm.tv/ep/:id/sort"] =
                                        vars.CurrentEpisode.Get(ctx)?.Sort.ToString() ?? "",
                                }),
                                InImageUrl = null,
                                InLinkAddress = new(ctx =>  new Uri($"https://bgm.tv/ep/{vars.CurrentEpisode.Get(ctx)?.Id}")),
                                InLinkMimeType = new("text/html"),
                            },
                        },
                    },
                },
            }
            }
        };
        var result = await _workflowHost.RunAsync(workflow, new RunWorkflowOptions()
        {
            Input = new Dictionary<string, object>
            {
                ["SubjectId"] = id,
            }
        });
        return result.Result as AppModels.Item ??
            throw new Exception(string.Join(", ", result.WorkflowState.Incidents.Select(x => x.Message)) ?? "Execution failed.");
    }

    public IAsyncEnumerable<LegacySubjectSmall> SearchAsync(string query)
    {
        return BgmApi.SearchAsync(query);
    }
}
