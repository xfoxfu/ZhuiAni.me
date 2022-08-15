using Me.Xfox.ZhuiAnime.External.Bangumi.Models;
using RestSharp;

namespace Me.Xfox.ZhuiAnime.External.Bangumi;

public partial class BangumiClient
{
    private RestClient Client { get; init; }

    public BangumiClient()
    {
        Client = new(new RestClientOptions("https://api.bgm.tv")
        {
            UserAgent = "xfoxfu/zhuianime (https://github.com/xfoxfu/ZhuiAni.me)"
        });
    }

    #region /v0/subjects
    public async Task<Subject> GetSubjectAsync(int id, CancellationToken ct = default)
    {
        var request = new RestRequest("/v0/subjects/{subject_id}")
            .AddUrlSegment("subject_id", id.ToString());
        var response = await Client.GetAsync<Subject>(request, ct);
        return response!;
    }
    #endregion
}
