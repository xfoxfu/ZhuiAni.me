using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using WorkflowCore.Interface;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class BangumiImportWorkflow : IWorkflow<BangumiImportWorkflow.Data>
{
    public string Id => "01HAV9B315QCQ2GVZD3N7WSDR0";

    public int Version => 1;

    public record class Data
    {
        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;
        public IEnumerable<Episode> Episodes { get; set; } = new List<Episode>();
        public IDictionary<uint, string> EpisodeFormattedNames { get; set; } = new Dictionary<uint, string>();
        public Ulid CategoryId { get; set; }
        public Ulid AnimeItemId { get; set; }
    }

    public void Build(IWorkflowBuilder<Data> builder)
    {
        builder
            .StartWith<GetBangumiSubject>()
                .Input(s => s.SubjectId, w => w.SubjectId)
                .Output(w => w.Subject, s => s.Subject)
                .Output(w => w.Episodes, s => s.Episodes)
            .Then<CreateCategory>()
                .Input(s => s.Title, x => "アニメ")
                .Output(w => w.CategoryId, s => s.Category.Id)
            .Then<CreateItemWithLink>()
                .Input(s => s.Name, w => w.Subject.Name)
                .Input(s => s.CategoryId, w => w.CategoryId)
                .Input(s => s.ImageUrl, w => w.Subject.Images.Large)
                .Input(s => s.LinkAddress, w => new Uri($"https://bgm.tv/subject/{w.Subject.Id}"))
                .Input(s => s.LinkMimeType, _ => "text/html;kind=bgm.tv")
                .Output(w => w.AnimeItemId, s => s.Item.Id)
            .ForEach(w => w.Episodes)
            .Do(x => x
                .StartWith<FormatEpisodeName>()
                    .Input(s => s.Episode, (w, ctx) => (Episode)ctx.Item)
                    .Input(s => s.TotalEpisodes, (w, ctx) => (uint)w.Subject.TotalEpisodes)
                    .Output((s, w) => w.EpisodeFormattedNames[s.Episode.Id] = s.FormattedName)
                .Then<CreateItemWithLink>()
                    .Input(s => s.Name, (w, ctx) => w.EpisodeFormattedNames[((Episode)ctx.Item).Id])
                    .Input(s => s.CategoryId, (w, ctx) => w.CategoryId)
                    .Input(s => s.ParentItemId, (w, ctx) => w.AnimeItemId)
                    .Input(s => s.ImageUrl, (w, ctx) => null)
                    .Input(s => s.LinkAddress, (w, ctx) => new Uri($"https://bgm.tv/ep/{((Episode)ctx.Item).Id}"))
                    .Input(s => s.LinkMimeType, (w, ctx) => "text/html;kind=bgm.tv")
                    // .Input(s => s.Annotations["https://bgm.tv/ep/:id/type"],
                    //     (w, ctx) => ((Episode)ctx.Item).Type == Episode.EpisodeType.SP ? "SP" : "Origin")
                    // .Input(s => s.Annotations["https://bgm.tv/ep/:id/sort"],
                    //     (w, ctx) => ((Episode)ctx.Item).Sort != null ? ((Episode)ctx.Item).Sort.ToString() : "")
                    .Output(w => w.AnimeItemId, s => s.Item.Id)
            );
    }
}
