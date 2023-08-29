using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;

namespace Me.Xfox.ZhuiAnime.Utils;

public class AuthenticationEventHandler : JwtBearerEvents
{
    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        var err = new ZhuiAnimeError.InvalidToken(
            context.Error,
            context.ErrorDescription,
            context.AuthenticateFailure
        );
        err.WithHttpContext(context.HttpContext);

        // From: https://github.com/dotnet/aspnetcore/blob/9402bfac90a695bb732dff17ba624801076df77f/src/Security/Authentication/JwtBearer/src/JwtBearerHandler.cs#L296-L345
        #region original logic
        context.Response.StatusCode = 401;

        if (string.IsNullOrEmpty(context.Error) &&
            string.IsNullOrEmpty(context.ErrorDescription) &&
            string.IsNullOrEmpty(context.ErrorUri))
        {
            context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);
        }
        else
        {
            // https://tools.ietf.org/html/rfc6750#section-3.1
            // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
            var builder = new StringBuilder(context.Options.Challenge);
            if (context.Options.Challenge.IndexOf(' ') > 0)
            {
                // Only add a comma after the first param, if any
                builder.Append(',');
            }
            if (!string.IsNullOrEmpty(context.Error))
            {
                builder.Append(" error=\"");
                builder.Append(context.Error);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(context.ErrorDescription))
            {
                if (!string.IsNullOrEmpty(context.Error))
                {
                    builder.Append(',');
                }

                builder.Append(" error_description=\"");
                builder.Append(context.ErrorDescription);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(context.ErrorUri))
            {
                if (!string.IsNullOrEmpty(context.Error) ||
                    !string.IsNullOrEmpty(context.ErrorDescription))
                {
                    builder.Append(',');
                }

                builder.Append(" error_uri=\"");
                builder.Append(context.ErrorUri);
                builder.Append('\"');
            }

            context.Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
        }
        #endregion

        // Customized logic for JSON response
        if (context.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new ZhuiAnimeError.ErrorDevResponse(err));
        }
        else
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new ZhuiAnimeError.ErrorProdResponse(err));
        }
    }
}
