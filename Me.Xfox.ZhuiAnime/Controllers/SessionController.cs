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
    private ZAContext DbContext { get; init; }
    protected TurnstileService TurnstileService { get; init; }
    protected TokenService TokenService { get; init; }

    public SessionController(ZAContext dbContext, TurnstileService turnstileService, TokenService tokenService)
    {
        DbContext = dbContext;
        TurnstileService = turnstileService;
        TokenService = tokenService;
    }

    public record LoginReqDto(
        string Username,
        string Password,
        string Captcha,
        string GrantType
    );

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
    /// Login with username and password. This API does not comply with OAuth 2.0,
    /// and only supports first-party applications (the built-in web frontend).
    /// </remarks>
    /// <param name="req"></param>
    /// <returns></returns>
    /// <exception cref="ZhuiAnimeError.InvalidGrantType">If the grant type is invalid.</exception>
    /// <exception cref="ZhuiAnimeError.InvalidUsernameOrPassword"></exception>
    [HttpPost]
    [AllowAnonymous]
    [ResponseCache(NoStore = true)]
    public async Task<LoginResDto> Login(LoginReqDto req)
    {
        await TurnstileService.Validate(req.Captcha);
        if (req.GrantType != "password")
        {
            throw new ZhuiAnimeError.InvalidGrantType(req.GrantType);
        }
        var user = await DbContext.User.FirstOrDefaultAsync(x => x.Username == req.Username);
        if (Models.User.ValidatePassword(user, req.Password) != true)
        {
            throw new ZhuiAnimeError.InvalidUsernameOrPassword(req.Username);
        }
        var (token, jwt) = TokenService.IssueFirstParty(user);
        var expires = jwt.Payload.Exp ?? 0;
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var scopes = jwt.Payload.Claims.FirstOrDefault(x => x.Type == TokenService.JwtClaimNames.Scope)?.Value ?? "";
        return new(
            AccessToken: token,
            ExpiresIn: (uint)(expires - now),
            RefreshToken: "",
            Scope: scopes);
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
