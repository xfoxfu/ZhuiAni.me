using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Management.Activities.SetOutput;
using Elsa.Workflows.Memory;
using Me.Xfox.ZhuiAnime.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using Me.Xfox.ZhuiAnime.Services.Workflow;

using AppModels = Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class ImportBangumiWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var vars = new
        {
            Subject = new Variable<Subject>(),
            Episodes = new Variable<IEnumerable<Episode>>(),
            AnimeCategory = new Variable<Category>(),
            AnimeItem = new Variable<AppModels.Item>(),
            CurrentEpisode = new Variable<Episode>(),
        };
        builder.WithVariables(vars.Subject, vars.Episodes, vars.AnimeCategory, vars.AnimeItem, vars.CurrentEpisode);
        builder.Result = vars.AnimeItem;
        builder.Root = new Sequence
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
                new SetOutput()
                {
                    OutputName = new("Result"),
                    OutputValue = new(vars.AnimeItem),
                },
            }
        };
    }
}
