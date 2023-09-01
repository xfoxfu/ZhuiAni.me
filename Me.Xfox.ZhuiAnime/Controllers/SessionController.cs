using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Operate users.
/// </summary>
[ApiController, Route("api/session")]
public class SessionController : ControllerBase
{
    protected ZAContext DbContext { get; init; }
    protected TurnstileService TurnstileService { get; init; }
    protected TokenService TokenService { get; init; }

    public SessionController(ZAContext dbContext, TurnstileService turnstileService, TokenService tokenService)
    {
        DbContext = dbContext;
        TurnstileService = turnstileService;
        TokenService = tokenService;
    }

    public record LoginReqDto
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string captcha { get; set; } = string.Empty;
        public string grant_type { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
    }

    public record LoginResDto(
        string AccessToken,
        uint ExpiresIn,
        string RefreshToken,
        string Scope,
        string TokenType = "Bearer",
        string IssuedTokenType = "urn:ietf:params:oauth:token-type:access_token"
    );

    /// <summary>Login</summary>
    /// <remarks><![CDATA[
    /// Login with username and password. This API does not comply with OAuth 2.1,
    /// and only supports first-party applications (the built-in web frontend).
    /// It is based on `grant_type` `password` (which has been drooped in OAuth 2.1)
    /// or `refresh_token`. It requires additional parameters for security control.
    /// 
    /// **Request with password**
    /// 
    /// It requires `username`, `password`, `captcha`.
    /// 
    /// ```text
    /// username=alice&password=foobar&captcha=foobar&grant_type=password
    /// ```
    /// 
    /// **Request with refresh token**
    /// 
    /// It requires `refresh_token`.
    /// 
    /// ```text
    /// grant_type=refresh_token&refresh_token=507f0155-577e-448d-870b-5abe98a41d3f
    /// ```
    /// ]]></remarks>
    /// <param name="req"></param>
    /// <returns></returns>
    /// <exception cref="ZhuiAnimeError.InvalidGrantType">If the grant type is invalid.</exception>
    /// <exception cref="ZhuiAnimeError.InvalidUsernameOrPassword"></exception>
    [HttpPost]
    [AllowAnonymous]
    [ResponseCache(NoStore = true)]
    [Consumes("application/x-www-form-urlencoded")]
    [ZhuiAnimeError.HasException(typeof(ZhuiAnimeError.InvalidGrantType), typeof(ZhuiAnimeError.InvalidUsernameOrPassword))]
    public async Task<LoginResDto> Login([FromForm] LoginReqDto req)
    {
        if (req.grant_type == "password")
        {
            await TurnstileService.Validate(req.captcha);
            var user = await DbContext.User.FirstOrDefaultAsync(x => x.Username == req.username);
            if (Models.User.ValidatePassword(user, req.password) != true)
            {
                throw new ZhuiAnimeError.InvalidUsernameOrPassword(req.username);
            }
            var refresh = await TokenService.IssueFirstPartyRefreshToken(user, null);
            var (token, jwt) = TokenService.IssueFirstParty(user, refresh);
            var expires = jwt.Payload.Exp ?? 0;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var scopes = jwt.Payload.Claims.FirstOrDefault(x => x.Type == TokenService.JwtClaimNames.Scope)?.Value ?? "";
            return new(
                AccessToken: token,
                ExpiresIn: (uint)(expires - now),
                RefreshToken: refresh.Token.ToString(),
                Scope: scopes);
        }
        else if (req.grant_type == "refresh_token")
        {
            if (!Guid.TryParse(req.refresh_token, out var tokenId))
            {
                throw new ZhuiAnimeError.InvalidRefreshToken("not_guid");
            }
            var refresh = await DbContext.Session
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == tokenId) ??
                throw new ZhuiAnimeError.InvalidRefreshToken("not_found");
            if (refresh.UserUpdatedAt != refresh.User.UpdatedAt)
            {
                throw new ZhuiAnimeError.InvalidRefreshToken("user_updated");
            }
            if (refresh.ExpiresIn < DateTimeOffset.Now)
            {
                throw new ZhuiAnimeError.InvalidRefreshToken("token_expired");
            }
            var newRefresh = await TokenService.IssueFirstPartyRefreshToken(refresh.User, refresh);
            var (token, jwt) = TokenService.IssueFirstParty(refresh.User, newRefresh);
            var expires = jwt.Payload.Exp ?? 0;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var scopes = jwt.Payload.Claims.FirstOrDefault(x => x.Type == TokenService.JwtClaimNames.Scope)?.Value ?? "";
            return new(
                AccessToken: token,
                ExpiresIn: (uint)(expires - now),
                RefreshToken: newRefresh.Token.ToString(),
                Scope: scopes);
        }
        else
        {
            throw new ZhuiAnimeError.InvalidGrantType(req.grant_type);
        }
    }

    public record TokenDto(
        UserController.UserDto User,
        DateTimeOffset IssuedAt,
        DateTimeOffset ExpiresAt
    );

    [HttpGet]
    public async Task<TokenDto> Get()
    {
        if (!TokenService.IsFirstParty(User))
        {
            throw new ZhuiAnimeError.InvalidTokenNotFirstParty();
        }
        var expires = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
        var issued = User.FindFirstValue(JwtRegisteredClaimNames.Iat);
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Convert.ToUInt32(subject);
        var user = await DbContext.User.FindAsync(userId);
        return new(
            new(user!),
            DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(issued)),
            DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expires))
        );
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        if (!TokenService.IsFirstParty(User))
        {
            throw new ZhuiAnimeError.InvalidTokenNotFirstParty();
        }
        var refresh = User.FindFirstValue(JwtRegisteredClaimNames.Sid) ??
            throw new ZhuiAnimeError.InvalidToken("za_sid_not_present", "no sid in token", null);
        var tokenId = Guid.Parse(refresh);
        var token = await DbContext.Session.FindAsync(tokenId) ??
            throw new ZhuiAnimeError.InvalidToken("za_refresh_token_not_found", $"refresh token {refresh} not found", null);
        DbContext.Session.Remove(token);
        await DbContext.SaveChangesAsync();
        return NoContent();
    }
}
