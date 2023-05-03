using System.Net;
using System.Text.Json;
using Me.Xfox.ZhuiAnime.External.Bangumi.Models;
using RestSharp;

namespace Me.Xfox.ZhuiAnime.External.Bangumi;

public class BangumiException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public Error? Error { get; set; }
    public bool IsNetworkError { get => StatusCode == 0; }

    public BangumiException(string message, Exception innerException) : base(message, innerException) { }

    public static BangumiException FromResponse(RestResponse response)
    {
        if (response.ErrorException == null && response.ErrorMessage == null && response.IsSuccessful)
        {
            throw new ArgumentException("cannot construct exception from success response", nameof(response));
        }

        System.Diagnostics.Debug.Assert(response.ErrorException != null, "ErrorException should not be null");

        if (response.StatusCode == 0)
        {
            return new BangumiException(
                response.ErrorMessage ?? response.ErrorException.Message,
                response.ErrorException);
        }

        var error = response.Content != null ? JsonSerializer.Deserialize<Error>(response.Content) : null;
        var message = error?.Description ?? response.StatusDescription;
        return new BangumiException(
            $"BangumiError ({response.StatusDescription}): {message}",
            response.ErrorException)
        {
            Error = error,
            StatusCode = response.StatusCode,
        };
    }
}
