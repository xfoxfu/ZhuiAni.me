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
}
