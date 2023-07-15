using System.Collections.Concurrent;
using System.Text.Json;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class PikPakClient
{
    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.Configure<Option>(builder.Configuration.GetSection(Option.LOCATION));
        return builder;
    }

    protected IOptionsMonitor<Option> Options { get; init; }

    protected Bangumi.BangumiService Bangumi { get; init; }

    private string Username => Options.CurrentValue.Username;
    private string Password => Options.CurrentValue.Password;
    private string AddressPrefix => Options.CurrentValue.AddressPrefix;

    protected string RefreshToken { get; set; } = string.Empty;
    protected string AccessToken { get; set; } = string.Empty;

    protected RestClient AuthClient { get; set; }
    protected RestClient Client { get; set; }

    protected TimeSpan FolderCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    public PikPakClient(IOptionsMonitor<Option> options, Bangumi.BangumiService bangumi)
    {
        Options = options;
        Bangumi = bangumi;
        AuthClient = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://user.mypikpak.com/v1"),
            UserAgent = "",
        });
        Client = new RestClient(new RestClientOptions
        {
            BaseUrl = new Uri("https://api-drive.mypikpak.com/drive/v1"),
            UserAgent = "",
            Authenticator = new JwtAuthenticator("INVALID_TOKEN"),
        });
    }

    public class Option
    {
        public const string LOCATION = "Modules:PikPak";

        public required string Username { get; set; }

        public required string Password { get; set; }

        public required string AddressPrefix { get; set; }
    }

    public async Task<Item> ImportBangumiSubject(uint id)
    {
        return await Bangumi.ImportSubject((int)id);
    }

    #region API
    public async Task<Types.LoginResponse> Login()
    {
        var req = new RestRequest("/auth/signin")
            .AddJsonBody(new Types.LoginRequest
            {
                CaptchaToken = "",
                ClientId = "YNxT9w7GMdWvEOKa",
                ClientSecret = "dbw2OtmVEeuUvIptb1Coyg",
                Username = Username,
                Password = Password,
            });
        var res = await AuthClient.PostAsync<Types.LoginResponse>(req);
        RefreshToken = res?.RefreshToken ?? throw new Exception("Login failed");
        AccessToken = res?.AccessToken ?? throw new Exception("Login failed");
        (Client.Options.Authenticator as JwtAuthenticator)?.SetBearerToken(AccessToken);
        return res ?? throw new Exception("Login failed");
    }

    public class PikPakException : Exception
    {
        public PikPakException(Types.PikPakError error, Exception inner) : base(error.Error, inner) { }
    }

    public async Task<Types.TaskResponse> Download(string url)
    {
        var req = new RestRequest("/files")
            .AddJsonBody(new Types.UploadRequest
            {
                Kind = "drive#file",
                FolderType = "DOWNLOAD",
                UploadType = "UPLOAD_TYPE_URL",
                Url = new Types.UploadRequest.DownloadRequestUrl
                {
                    Url = url,
                },
            });
        var res = await Client.PostAsync<Types.UploadResponse>(req);
        return res?.Task ?? throw new Exception("API request failed");
    }

    public async Task<Types.TaskResponse> Download(string url, string parentId)
    {
        var req = new RestRequest("/files")
            .AddJsonBody(new Types.UploadRequest
            {
                Kind = "drive#file",
                ParentId = parentId,
                UploadType = "UPLOAD_TYPE_URL",
                Url = new Types.UploadRequest.DownloadRequestUrl { Url = url },
            });
        var res = await Client.PostAsync<Types.UploadResponse>(req);
        return res?.Task ?? throw new Exception("API request failed");
    }

    public async Task<Types.FileResponse> CreateFolder(string name, string? parentId)
    {
        var req = new RestRequest("/files")
            .AddJsonBody(new Types.UploadRequest
            {
                Kind = "drive#folder",
                Name = name,
                ParentId = parentId,
            });
        var res = await Client.PostAsync<Types.UploadResponse>(req);
        return res?.File ?? throw new Exception("API request failed");
    }

    public async Task<bool> WaitForTask(string taskId)
    {
        return await WaitForTask(taskId, TimeSpan.FromSeconds(1), 10);
    }

    public async Task<bool> WaitForTask(string taskId, TimeSpan interval)
    {
        return await WaitForTask(taskId, interval, 10);
    }

    public async Task<bool> WaitForTask(string taskId, uint threshold)
    {
        return await WaitForTask(taskId, TimeSpan.FromSeconds(1), threshold);
    }

    public async Task<bool> WaitForTask(string taskId, TimeSpan interval, uint threshold)
    {
        var req = new RestRequest($"/tasks/{taskId}");
        var res = await Client.GetAsync<Types.TaskResponse>(req);
        while (res?.Phase != "PHASE_TYPE_COMPLETE" && res?.Phase != "PHASE_TYPE_ERROR" && threshold > 0)
        {
            threshold -= 1;
            await Task.Delay(interval);
            res = await Client.GetAsync<Types.TaskResponse>(req);
        }
        return res?.Phase == "PHASE_TYPE_COMPLETE" || res?.Phase == "PHASE_TYPE_ERROR";
    }

    public async Task<IEnumerable<Types.FileResponse>> List(string? folderId)
    {
        var req = new RestRequest($"/files");
        if (!string.IsNullOrEmpty(folderId))
        {
            req = req.AddQueryParameter("parent_id", folderId);
        }
        var res = await Client.GetAsync<Types.ListFileResponse>(req);
        return res?.Files ?? throw new Exception("API request failed");
    }

    public async Task<Types.FileResponse> GetFile(string fileId)
    {
        var req = new RestRequest($"/files/{fileId}", Method.Get);
        var res = await Client.ExecuteGetAsync<Types.FileResponse>(req);
        return res.Data ?? throw new Exception("API request failed");
    }

    public async Task<Types.TaskResponse> MoveFile(string fileId, string parentId)
    {
        var req = new RestRequest($"/files/{fileId}/move")
            .AddJsonBody(new Types.MoveRequest
            {
                Ids = new[] { fileId },
                To = new Types.MoveRequest.MoveTarget { ParentId = parentId },
            });
        var res = await Client.PostAsync<Types.TaskResponse>(req);
        return res ?? throw new Exception("API request failed");
    }
    #endregion

    public async Task EnsureLogin()
    {
        if (string.IsNullOrEmpty(RefreshToken) || string.IsNullOrEmpty(AccessToken))
        {
            await Login();
        }
    }

    protected ConcurrentDictionary<string, string> Cache { get; } = new();

    public async Task<Types.FileResponse> ResolveFolder(IEnumerable<string> path)
    {
        await GetFile("INVALID");
        if (Cache.TryGetValue(string.Join('/', path), out var cachedFileId))
        {
            var cachedFile = await GetFile("INVALID");
            if (cachedFile != null && !cachedFile.Trashed) return cachedFile;
        }

        Types.FileResponse? file = null;
        if (!path.Any()) throw new Exception("Path is empty");
        foreach (var seg in path)
        {
            var parent = file?.Id;
            var files = await List(parent);
            file = files.FirstOrDefault(f => f.Name == seg && !f.Trashed);
            file ??= await CreateFolder(seg, parent);
        }
        Cache[string.Join('/', path)] = file?.Id!;
        return file!;
    }

    public async Task<Types.FileResponse> DownloadLink(string url, IEnumerable<string> path)
    {
        await EnsureLogin();
        var file = await ResolveFolder(path);
        var task = await Download(url, file.Id);
        if (!await WaitForTask(task.Id))
        {
            throw new Exception("Download failed");
        }
        return await GetFile(task.FileId);
    }
}
