using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class GetBangumiSubject : StepBodyAsync
{
    protected BangumiService _bangumiService;

    public GetBangumiSubject(BangumiService bangumiService)
    {
        _bangumiService = bangumiService;
    }

    public int SubjectId { get; set; }

    public Subject Subject { get; set; } = null!;
    public IEnumerable<Episode> Episodes { get; set; } = null!;

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        Subject = await _bangumiService.GetSubject(SubjectId);
        Episodes = await _bangumiService.GetEpisodes(SubjectId);
        return ExecutionResult.Next();
    }
}
