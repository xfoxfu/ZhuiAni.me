using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class GetBangumiSubject : CodeActivity
{
    public required Input<int> InSubjectId { get; set; }

    public Output<Subject>? OutSubject { get; set; }
    public Output<IEnumerable<Episode>>? OutEpisodes { get; set; }

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var _bangumiService = context.GetRequiredService<BangumiService>();
        var SubjectId = InSubjectId.Get(context);

        OutSubject.Set(context, await _bangumiService.GetSubject(SubjectId));
        OutEpisodes.Set(context, await _bangumiService.GetEpisodes(SubjectId));
    }
}
