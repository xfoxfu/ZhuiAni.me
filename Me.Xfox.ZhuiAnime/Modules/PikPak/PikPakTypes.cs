using System.Text.Json.Serialization;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak.Types;

public class LoginRequest
{
    [JsonPropertyName("captcha_token")]
    public required string CaptchaToken { get; set; }

    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }

    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("password")]
    public required string Password { get; set; }
}

public class LoginResponse
{
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }

    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
}

public class UploadRequest
{
    [JsonPropertyName("kind")]
    public required string Kind { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("upload_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UploadType { get; set; }

    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DownloadRequestUrl? Url { get; set; }

    [JsonPropertyName("parent_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentId { get; set; }

    [JsonPropertyName("folder_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FolderType { get; set; }

    public class DownloadRequestUrl
    {
        [JsonPropertyName("url")]
        public required string Url { get; set; }
    }
}

public class UploadResponse
{
    [JsonPropertyName("task")]
    public TaskResponse? Task { get; set; }

    [JsonPropertyName("file")]
    public FileResponse? File { get; set; }
}

public class TaskResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("file_id")]
    public required string FileId { get; set; }

    [JsonPropertyName("file_name")]
    public required string FileName { get; set; }

    [JsonPropertyName("phase")]
    public required string Phase { get; set; }
}

public class FileResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("parent_id")]
    public required string ParentId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("trashed")]
    public required bool Trashed { get; set; }
}

public class MoveRequest
{
    [JsonPropertyName("ids")]
    public required IEnumerable<string> Ids { get; set; }

    [JsonPropertyName("to")]
    public required MoveTarget To { get; set; }


    public class MoveTarget
    {
        [JsonPropertyName("parent_id")]
        public required string ParentId { get; set; }
    }
}

public class ListFileResponse
{
    [JsonPropertyName("files")]
    public required IEnumerable<FileResponse> Files { get; set; }
}

public class PikPakError
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }

    [JsonPropertyName("error_code")]
    public required int ErrorCode { get; set; }

    [JsonPropertyName("error_url")]
    public required string ErrorUrl { get; set; }

    [JsonPropertyName("error_description")]
    public required string ErrorDescription { get; set; }

    [JsonPropertyName("error_details")]
    public required IEnumerable<ErrorDetail> ErrorDetails { get; set; }

    public class ErrorDetail
    {
        [JsonPropertyName("@type")]
        public required string Type { get; set; }

        [JsonPropertyName("detail")]
        public required string Detail { get; set; }
    }
}
