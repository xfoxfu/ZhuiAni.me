using Elsa.Workflows.Management.Contracts;
using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Requests;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;
using AppModels = Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

public class BangumiService
{
    protected ZAContext DbContext { get; init; }
    protected Client.BangumiApi BgmApi { get; init; }
    protected readonly IWorkflowRuntime _workflowRuntime;
    protected readonly IWorkflowExecutionLogStore _log;
    protected readonly IWorkflowInstanceStore _store;
    protected readonly IActivityExecutionStore _activityStore;

    public BangumiService(
        Client.BangumiApi bgmApi,
        ZAContext dbContext,
        IWorkflowRuntime workflowRuntime,
        IWorkflowExecutionLogStore log,
        IWorkflowInstanceStore store,
        IActivityExecutionStore activityStore)
    {
        BgmApi = bgmApi;
        DbContext = dbContext;
        _workflowRuntime = workflowRuntime;
        _log = log;
        _store = store;
        _activityStore = activityStore;
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
        var result = await _workflowRuntime.StartWorkflowAsync(nameof(ImportBangumiWorkflow), new StartWorkflowRuntimeOptions()
        {
            Input = new Dictionary<string, object>
            {
                ["SubjectId"] = id,
            },
        });
        var state = await _workflowRuntime.ExportWorkflowStateAsync(result.WorkflowInstanceId);
        if ((state?.Output.TryGetValue("Result", out var item)) != true || item == null)
        {
            throw new Exception("Workflow execution failed");
        }
        return (AppModels.Item)item;
    }

    public IAsyncEnumerable<LegacySubjectSmall> SearchAsync(string query)
    {
        return BgmApi.SearchAsync(query);
    }
}
