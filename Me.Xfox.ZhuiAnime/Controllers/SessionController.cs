using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Me.Xfox.ZhuiAnime.Models;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

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
        string Token
    );


    [HttpPost]
    [AllowAnonymous]
    public async Task<LoginResDto> Login(LoginReqDto req)
    {
        if (req.GrantType != "password")
        {
            throw new ZhuiAnimeError.InvalidGrantType(req.GrantType);
        }
        var user = await DbContext.User.FirstOrDefaultAsync(x => x.Username == req.Username);
        if (Models.User.ValidatePassword(user, req.Password))
        {
            throw new ZhuiAnimeError.InvalidUsernameOrPassword(req.Username);
        }
        return new(TokenService.IssueFirstParty(user!));
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
        var expires = User.FindFirstValue(JwtRegisteredClaimNames.Exp) ??
            throw new ZhuiAnimeError.InvalidToken("no_expires", "no exp in token", null);
        var issued = User.FindFirstValue(JwtRegisteredClaimNames.Iat) ??
            throw new ZhuiAnimeError.InvalidToken("no_issued_at", "no iat in token", null);
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new ZhuiAnimeError.InvalidToken("no_subject", "no sub in token", null);
        var userId = Convert.ToUInt32(subject);
        var user = await DbContext.User.FindAsync(userId) ??
            throw new ZhuiAnimeError.InvalidToken("user_not_found", "user not found", null);
        return new(
            new(user),
            DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(issued)),
            DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expires))
        );
    }
}
