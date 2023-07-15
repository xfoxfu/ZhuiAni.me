using System.Net;
using Flurl.Http;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class PikPakException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public Types.PikPakError? Error { get; set; }

    public PikPakException(string message, FlurlHttpException innerException) : base(message, innerException) { }

    public static async Task<PikPakException> FromResponse(FlurlHttpException response)
    {
        // HTTP network exception
        if (response.StatusCode == null
            || response.StatusCode < 300
            || response is FlurlHttpTimeoutException
            || response is FlurlParsingException
        ) throw response;

        try
        {
            var error = await response.GetResponseJsonAsync<Types.PikPakError>();
            return new PikPakException($"PikPak Error ({response.StatusCode}): {error.Error}", response)
            {
                Error = error,
                StatusCode = (HttpStatusCode)response.StatusCode,
            };
        }
        catch (Exception e)
        {
            throw new Exception("PikPak responded with HTTP error but JSON deserialization failed.", e);
        }
    }
}

public static class PikPakExceptionExtensions
{
    public static IFlurlClient UsePikPakExceptionHandler(this IFlurlClient client)
    {
        return client.OnError(async call =>
            {
                var exception = call.Exception as FlurlHttpException ?? new FlurlHttpException(call, call.Exception);
                throw await PikPakException.FromResponse(exception);
            });
    }
}
