using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Flurl.Http;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.Extensions.Options;
using static Me.Xfox.ZhuiAnime.Models.Link.CommonAnnotations;

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

    protected ILogger<PikPakClient> Logger { get; init; }

    private string Username => Options.CurrentValue.Username;
    private string Password => Options.CurrentValue.Password;
    private string AccessAddressTemplate => Options.CurrentValue.AccessAddressTemplate;

    protected string RefreshToken { get; set; } = string.Empty;
    protected string AccessToken { get; set; } = string.Empty;

    protected IFlurlClient AuthClient { get; set; }
    protected IFlurlClient Client { get; set; }

    protected TimeSpan FolderCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    public PikPakClient(IOptionsMonitor<Option> options, Bangumi.BangumiService bangumi, ILogger<PikPakClient> logger)
    {
        Options = options;
        Bangumi = bangumi;
        AuthClient = new FlurlClient("https://user.mypikpak.com/v1")
            .WithHeader("User-Agent", "")
            .UsePikPakExceptionHandler();
        Client = new FlurlClient("https://api-drive.mypikpak.com/drive/v1")
            .WithHeader("User-Agent", "")
            .UsePikPakExceptionHandler();
        Logger = logger;
    }

    public class Option
    {
        public const string LOCATION = "Modules:PikPak";

        public required string Username { get; set; }

        public required string Password { get; set; }

        public required string AccessAddressTemplate { get; set; }

        public required TimeSpan IntervalBetweenFetch { get; set; }
    }

    public async Task<uint> ImportBangumiSubject(uint id)
    {
        Logger.LogInformation("Importing Bangumi subject {Id}", id);
        return await Bangumi.ImportSubjectGetId((int)id);
    }

    #region API
    public async Task<Types.LoginResponse> Login()
    {
        Logger.LogInformation("Log in to PikPak.");
        var res = await AuthClient.Request("/auth/signin")
            .PostJsonAsync(new Types.LoginRequest
            {
                CaptchaToken = "",
                ClientId = "YNxT9w7GMdWvEOKa",
                ClientSecret = "dbw2OtmVEeuUvIptb1Coyg",
                Username = Username,
                Password = Password,
            })
            .ReceiveJson<Types.LoginResponse>();
        RefreshToken = res.RefreshToken;
        AccessToken = res.AccessToken;
        Client = Client.WithOAuthBearerToken(AccessToken);
        return res;
    }

    // TODO: use refresh_token
    public async Task<Types.LoginResponse> RefreshLogin()
    {
        var res = await AuthClient.Request("/auth/token")
            .PostJsonAsync(new Types.RefreshLoginRequest
            {
                ClientId = "YNxT9w7GMdWvEOKa",
                ClientSecret = "dbw2OtmVEeuUvIptb1Coyg",
                GrantType = "refresh_token",
                RefreshToken = RefreshToken,
            })
            .ReceiveJson<Types.LoginResponse>();
        RefreshToken = res.RefreshToken;
        AccessToken = res.AccessToken;
        Client = Client.WithOAuthBearerToken(AccessToken);
        return res;
    }

    public async Task<Types.TaskResponse> Download(string url)
    {
        var res = await Client.Request("/files")
            .PostJsonAsync(new Types.UploadRequest
            {
                Kind = "drive#file",
                FolderType = "DOWNLOAD",
                UploadType = "UPLOAD_TYPE_URL",
                Url = new Types.UploadRequest.DownloadRequestUrl
                {
                    Url = url,
                },
            })
            .ReceiveJson<Types.UploadResponse>();
        return res.Task!;
    }

    public async Task<Types.TaskResponse> Download(string url, string parentId)
    {
        var res = await Client.Request("/files")
            .PostJsonAsync(new Types.UploadRequest
            {
                Kind = "drive#file",
                ParentId = parentId,
                UploadType = "UPLOAD_TYPE_URL",
                Url = new Types.UploadRequest.DownloadRequestUrl { Url = url },
            })
            .ReceiveJson<Types.UploadResponse>();
        return res.Task!;
    }

    public async Task<Types.FileResponse> CreateFolder(string name, string? parentId)
    {
        Logger.LogInformation("Creating folder {Name} in {ParentId}", name, parentId);
        var res = await Client.Request("/files")
            .PostJsonAsync(new Types.UploadRequest
            {
                Kind = "drive#folder",
                Name = name,
                ParentId = parentId,
            })
            .ReceiveJson<Types.UploadResponse>();
        return res.File!;
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
        var res = await Client.Request($"/tasks/{taskId}").GetJsonAsync<Types.TaskResponse>();
        while (res.Phase != "PHASE_TYPE_COMPLETE" && res.Phase != "PHASE_TYPE_ERROR" && threshold > 0)
        {
            threshold -= 1;
            await Task.Delay(interval);
            res = await Client.Request($"/tasks/{taskId}").GetJsonAsync<Types.TaskResponse>();
        }
        return res.Phase == "PHASE_TYPE_COMPLETE" || res.Phase == "PHASE_TYPE_ERROR";
    }

    public async Task<IEnumerable<Types.FileResponse>> List(string? folderId)
    {
        var res = await Client.Request($"/files")
            .SetQueryParam("parent_id", folderId)
            .GetJsonAsync<Types.ListFileResponse>();
        return res.Files;
    }

    public async Task<Types.FileResponse> GetFile(string fileId)
    {
        var res = await Client.Request("/files", fileId).GetJsonAsync<Types.FileResponse>();
        return res;
    }

    public async Task<Types.TaskResponse> MoveFile(string fileId, string parentId)
    {
        var res = await Client.Request($"/files/{fileId}/move")
            .PostJsonAsync(new Types.MoveRequest
            {
                Ids = new[] { fileId },
                To = new Types.MoveRequest.MoveTarget { ParentId = parentId },
            })
            .ReceiveJson<Types.TaskResponse>();
        return res;
    }
    #endregion

    public async Task EnsureLogin()
    {
        if (string.IsNullOrEmpty(RefreshToken) || string.IsNullOrEmpty(AccessToken))
        {
            await Login();
        }
        try
        {
            await List(null);
        }
        catch (PikPakException)
        {
            await Login();
        }
    }

    protected ConcurrentDictionary<string, string> Cache { get; } = new();

    public async Task<Types.FileResponse> ResolveFolder(IEnumerable<string> path)
    {
        if (Cache.TryGetValue(string.Join('/', path), out var cachedFileId))
        {
            try
            {
                var cachedFile = await GetFile(cachedFileId);
                if (!cachedFile.Trashed) return cachedFile;
            }
            catch (PikPakException e)
            {
                if (e.Error?.Error != "file_not_found") throw e;
            }
        }

        Logger.LogInformation("Resolving path {Path}", string.Join('/', path));
        Types.FileResponse? file = null;
        if (!path.Any()) throw new Exception("Path is empty");
        foreach (var seg in path)
        {
            Logger.LogDebug("Resolving path segment {Segment}", seg);
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

    public async Task<Link> ImportLink(Anime config, TorrentDirectory.Torrent torrent, ZAContext db, Item anime)
    {
        var source = (torrent.LinkMagnet ?? torrent.LinkTorrent) ?? throw new ArgumentNullException(
                message: $"{nameof(torrent)} does not have LinkMagnet nor LinkTorrent.", null);

        // Matching item in database
        var ep = Regex.Match(torrent.Title, config.Regex).Groups[(int)config.MatchGroup.Ep].Value;
        await db.Entry(anime).Collection(i => i.ChildItems!).LoadAsync();
        var episode = anime.ChildItems
            ?.FirstOrDefault(i => double.Parse(i.Annotations["https://bgm.tv/ep/:id/sort"]) == double.Parse(ep));

        // Check if link exists
        Link? existing = null;
        if (episode != null)
        {
            await db.Entry(episode).Collection(i => i.Links!).LoadAsync();
            existing = episode.Links!
                .FirstOrDefault(l => l.Annotations.TryGetValue(PikPakTorrentAddress, out var addr) && addr == source);
        }
        if (existing == null)
        {
            await db.Entry(anime).Collection(i => i.Links!).LoadAsync();
            existing = anime.Links!
                .FirstOrDefault(l => l.Annotations.TryGetValue(PikPakTorrentAddress, out var addr) && addr == source);
        }
        if (existing != null)
        {
            Logger.LogInformation(
                "Using existing {Link} for Anime {Anime}, Torrent {Torrent}.", existing.Id, config.Id, torrent.Id);
            config.LastFetchedAt = torrent.PublishedAt;
            await db.SaveChangesAsync();
            return existing;
        }

        // Download torrent
        var path = config.Target.Split("/").Where(x => !string.IsNullOrWhiteSpace(x));
        var file = await DownloadLink(source, path);
        var addr = Flurl.Url.Combine(path.Append(file.Name).ToArray());
        var linkAddress = string.Format(AccessAddressTemplate, addr);

        // Add link
        var link = new Link
        {
            ItemId = episode?.Id ?? anime.Id,
            Address = new Uri(linkAddress),
            MimeType = file.MimeType,
            Annotations = new Dictionary<string, string>
            {
                [PikPakFileId] = file.Id,
                [PikPakTorrentAddress] = source,
            }
        };
        db.Link.Add(link);
        config.LastFetchedAt = torrent.PublishedAt;
        await db.SaveChangesAsync();

        return link;
    }
}
