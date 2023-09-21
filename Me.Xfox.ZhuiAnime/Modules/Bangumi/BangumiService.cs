using System.Collections;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;
using Microsoft.AspNetCore.Mvc;
using WorkflowCore.Interface;
using AppModels = Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

public class BangumiService
{
    protected ZAContext DbContext { get; init; }
    protected Client.BangumiApi BgmApi { get; init; }
    protected readonly IWorkflowHost _workflowHost;
    protected readonly IPersistenceProvider _workflowStore;

    public BangumiService(Client.BangumiApi bgmApi, ZAContext dbContext, IWorkflowHost workflowHost, IPersistenceProvider workflowStore)
    {
        BgmApi = bgmApi;
        DbContext = dbContext;
        _workflowHost = workflowHost;
        _workflowStore = workflowStore;
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
        var workflowRunId = await _workflowHost.StartWorkflow("01HAV9B315QCQ2GVZD3N7WSDR0", new BangumiImportWorkflow.Data()
        {
            SubjectId = id
        });
        var instance = await _workflowStore.GetWorkflowInstance(workflowRunId);
        while (instance.Status == WorkflowCore.Models.WorkflowStatus.Runnable)
        {
            await Task.Delay(1000);
            instance = await _workflowStore.GetWorkflowInstance(workflowRunId);
        }
        var itemId = ((BangumiImportWorkflow.Data)instance.Data).AnimeItemId;
        return await DbContext.Item.FindAsync(itemId) ?? throw new Exception();
    }

    public IAsyncEnumerable<LegacySubjectSmall> SearchAsync(string query)
    {
        return BgmApi.SearchAsync(query);
    }
}
