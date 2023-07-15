using System.Net;
using System.Text.Json;
using Flurl.Http;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Client;

public class BangumiException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public Error? Error { get; set; }

    public BangumiException(string message, FlurlHttpException innerException) : base(message, innerException) { }

    public static async Task<BangumiException> FromResponse(FlurlHttpException response)
    {
        // HTTP network exception
        if (response.StatusCode == null || response.StatusCode < 300) throw response;
        try
        {
            var error = await response.GetResponseJsonAsync<Error>();
            return new BangumiException($"BangumiError ({response.StatusCode}): {error.Title}", response)
            {
                Error = error,
                StatusCode = (HttpStatusCode)response.StatusCode,
            };
        }
        catch (Exception e)
        {
            throw new Exception("Bangumi responded with HTTP error but JSON deserialization failed.", e);
        }
    }
}
