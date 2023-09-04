using System.Runtime.CompilerServices;
using Flurl.Http;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Client;

public partial class BangumiApi
{
    protected const string DEFAULT_BANGUMI_API_HOST = "https://api.bgm.tv/";
    protected const string DEFUALT_USER_AGENT = "xfoxfu/zhuianime (https://github.com/xfoxfu/ZhuiAni.me)";

    private IFlurlClient Client { get; init; }

    public BangumiApi(
        string baseUrl = DEFAULT_BANGUMI_API_HOST,
        string userAgent = DEFUALT_USER_AGENT)
    {
        Client = new FlurlClient(baseUrl)
            .WithHeader("User-Agent", userAgent)
            .WithHeader("Accept", "application/json")
            .UseBangumiExceptionHandler();
    }

    #region /v0/subjects
    public async Task<Subject> GetSubjectAsync(int id, CancellationToken ct = default)
    {
        return await Client
            .Request("/v0/subjects/", id)
            .GetJsonAsync<Subject>(cancellationToken: ct);
    }
    #endregion

    #region /v0/episodes
    public async IAsyncEnumerable<Episode> GetEpisodesAsync(
        int subjectId,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        uint offset = 0;
        uint total;
        do
        {
            if (ct.IsCancellationRequested) yield break;
            var response = await Client.Request("/v0/episodes")
                .SetQueryParam("subject_id", subjectId)
                .SetQueryParam("offset", offset)
                .GetJsonAsync<PaginatedResult<Episode>>(cancellationToken: ct);
            foreach (var episode in response.Data)
            {
                yield return episode;
            }
            offset = response.Offset + response.Limit;
            total = response.Total;
        } while (offset < total);
    }
    #endregion

    #region /search/subject
    public async IAsyncEnumerable<LegacySubjectSmall> SearchAsync(string query, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var request = await Client.Request("/search/subject")
            .AppendPathSegment(query, fullyEncode: true)
            .SetQueryParam("type", "2")
            .SetQueryParam("responseGroup", "small")
            .GetAsync(cancellationToken: ct);
        if (request.Headers.TryGetFirst("content-type", out var contentType) && !contentType.Contains("application/json"))
        {
            yield break;
        }
        var body = await request.GetJsonAsync<SearchResult<LegacySubjectSmall>>();
        foreach (var item in body.Items)
        {
            yield return item;
        }
    }
    #endregion
}
