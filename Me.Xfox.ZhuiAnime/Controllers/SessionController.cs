using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Captcha { get; set; } = string.Empty;
        public string Grant_Type { get; set; } = string.Empty;
        public string Refresh_Token { get; set; } = string.Empty;
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
    /// <remarks>
    /// Login with username and password. This API does not comply with OAuth 2.1,
    /// and only supports first-party applications (the built-in web frontend).
    /// It is based on grant_type password (which has been drooped in OAuth 2.1)
    /// or refresh_token. It requires additional parameters for security control.
    /// </remarks>
    /// <param name="req"></param>
    /// <returns></returns>
    /// <exception cref="ZhuiAnimeError.InvalidGrantType">If the grant type is invalid.</exception>
    /// <exception cref="ZhuiAnimeError.InvalidUsernameOrPassword"></exception>
    [HttpPost]
    [AllowAnonymous]
    [ResponseCache(NoStore = true)]
    public async Task<LoginResDto> Login([FromForm] LoginReqDto req)
    {
        if (req.Grant_Type == "password")
        {
            await TurnstileService.Validate(req.Captcha);
            var user = await DbContext.User.FirstOrDefaultAsync(x => x.Username == req.Username);
            if (Models.User.ValidatePassword(user, req.Password) != true)
            {
                throw new ZhuiAnimeError.InvalidUsernameOrPassword(req.Username);
            }
            var (token, jwt) = TokenService.IssueFirstParty(user);
            var refresh = await TokenService.IssueFirstPartyRefreshToken(user, null);
            var expires = jwt.Payload.Exp ?? 0;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var scopes = jwt.Payload.Claims.FirstOrDefault(x => x.Type == TokenService.JwtClaimNames.Scope)?.Value ?? "";
            return new(
                AccessToken: token,
                ExpiresIn: (uint)(expires - now),
                RefreshToken: refresh.Token.ToString(),
                Scope: scopes);
        }
        else if (req.Grant_Type == "refresh_token")
        {
            if (!Guid.TryParse(req.Refresh_Token, out var tokenId))
            {
                throw new ZhuiAnimeError.InvalidRefreshToken("not_guid");
            }
            var refresh = await DbContext.RefreshToken
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
            var (token, jwt) = TokenService.IssueFirstParty(refresh.User);
            var newRefresh = await TokenService.IssueFirstPartyRefreshToken(refresh.User, refresh);
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
            throw new ZhuiAnimeError.InvalidGrantType(req.Grant_Type);
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
}
