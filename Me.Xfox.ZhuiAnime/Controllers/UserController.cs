using Me.Xfox.ZhuiAnime.Models;
using Me.Xfox.ZhuiAnime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Operate users.
/// </summary>
[ApiController, Route("api/users")]
public class UserController : ControllerBase
{
    private ZAContext DbContext { get; init; }
    protected TurnstileService TurnstileService { get; init; }

    public UserController(ZAContext dbContext, TurnstileService turnstileService)
    {
        DbContext = dbContext;
        TurnstileService = turnstileService;
    }

    public record UserDto
    {
        public uint Id { get; init; }
        public string Username { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }

        public UserDto(User user)
        {
            Id = user.Id;
            Username = user.Username;
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;
        }
    }

    [HttpGet]
    public async Task<IEnumerable<UserDto>> List()
    {
        return await DbContext.User.Select(x => new UserDto(x)).ToListAsync();
    }

    public record CreateUserDto(
        string Username,
        string Password,
        string Captcha
    );

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ZAError.Has<ZAError.UsernameTaken>]
    public async Task<ActionResult<UserDto>> Register(CreateUserDto req)
    {
        await TurnstileService.Validate(req.Captcha);
        if (await DbContext.User.AnyAsync(x => x.Username == req.Username))
        {
            throw new ZAError.UsernameTaken(req.Username);
        }

        var user = new User
        {
            Username = req.Username,
            Password = req.Password
        };

        DbContext.User.Add(user);
        await DbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, new UserDto(user));
    }

    [HttpGet("{id}")]
    [ZAError.Has<ZAError.UserNotFound>]
    public async Task<UserDto> Get(uint id)
    {
        return new UserDto(await DbContext.User.FindAsync(id) ?? throw new ZAError.UserNotFound(id));
    }
}
