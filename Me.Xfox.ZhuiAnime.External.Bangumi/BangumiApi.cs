using System.Text.Json;
using Me.Xfox.ZhuiAnime.External.Bangumi.Models;
using RestSharp;

namespace Me.Xfox.ZhuiAnime.External.Bangumi;

public partial class BangumiApi
{
    protected const string DEFAULT_BANGUMI_API_HOST = "https://api.bgm.tv/";
    protected const string DEFUALT_USER_AGENT = "xfoxfu/zhuianime (https://github.com/xfoxfu/ZhuiAni.me)";

    private RestClient Client { get; init; }
    public string UserAgent
    {
        get => Client.Options.UserAgent;
        set => Client.Options.UserAgent = value;
    }

    public BangumiApi(
        string baseUrl = DEFAULT_BANGUMI_API_HOST,
        string userAgent = DEFUALT_USER_AGENT)
    {
        Client = new(new RestClientOptions(baseUrl)
        {
            UserAgent = userAgent,
        });
    }

    #region /v0/subjects
    public async Task<Subject> GetSubjectAsync(int id, CancellationToken ct = default)
    {
        var request = new RestRequest("/v0/subjects/{subject_id}", Method.Get)
            .AddUrlSegment("subject_id", id);
        return await GetResponseAsync<Subject>(request, ct);
    }
    #endregion

    #region utils
    public async Task<T> GetResponseAsync<T>(RestRequest request, CancellationToken ct = default)
    {
        var response = await Client.ExecuteAsync<T>(request, ct);
        if (response.IsSuccessful)
        {
            System.Diagnostics.Debug.Assert(response.Data != null);
            return response.Data;
        }

        throw BangumiException.FromResponse(response);
    }

    public async Task<byte[]> GetBytesAsync(RestRequest request, CancellationToken ct = default)
    {
        var response = await Client.GetAsync(request, ct);
        return response.RawBytes!;
    }

    public async Task<byte[]> GetBytesAsync(string url, CancellationToken ct = default)
        => await GetBytesAsync(new RestRequest(url), ct);
    #endregion
}
