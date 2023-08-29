using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Me.Xfox.ZhuiAnime.Services;

public class TokenService
{
    protected ILogger<TokenService> Logger { get; init; }
    protected IOptionsMonitor<Option> Options { get; set; }

    public TokenService(ILogger<TokenService> logger, IOptionsMonitor<Option> options)
    {
        Logger = logger;
        Options = options;
    }

    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.Configure<Option>(builder.Configuration.GetSection(Option.LOCATION));
        builder.Services.AddSingleton<TokenService>();
        return builder;
    }

    public string IssueFirstParty(Models.User user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtClaimNames.Scope, JwtScopes.OAuth),
            new (JwtClaimNames.UpdatedAt, user.UpdatedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };
        var key = ECDsa.Create();
        key.ImportFromPem(Options.CurrentValue.PrivateKey);
        var secKey = new ECDsaSecurityKey(key);
        var creds = new SigningCredentials(secKey, key.KeySize switch
        {
            256 => SecurityAlgorithms.EcdsaSha256,
            384 => SecurityAlgorithms.EcdsaSha384,
            521 => SecurityAlgorithms.EcdsaSha512,
            _ => throw new NotSupportedException(),
        });
        var token = new JwtSecurityToken(
            issuer: Options.CurrentValue.Issuer,
            audience: Options.CurrentValue.AudienceFirstParty,
            expires: DateTime.Now.Add(Options.CurrentValue.FirstPartyExpires),
            notBefore: DateTime.Now,
            claims: claims,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool IsFirstParty(ClaimsPrincipal user)
    {
        return user.FindAll(JwtClaimNames.Scope)
                .Any(x => x.Value == JwtScopes.OAuth) &&
            user.FindAll(JwtRegisteredClaimNames.Aud)
                .Any(x => x.Value == Options.CurrentValue.AudienceFirstParty);
    }

    public struct JwtClaimNames
    {
        public const string Scope = "scope";
        public const string UpdatedAt = "updated_at";
    }

    public struct JwtScopes
    {
        public const string OAuth = "1p_oauth";
    }

    public class Option
    {
        public const string LOCATION = "Authentication:Jwt";

        public string PrivateKey { get; set; } = string.Empty;

        public string PublicKey { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string AudienceFirstParty { get; set; } = string.Empty;

        public TimeSpan FirstPartyExpires { get; set; }

        public SecurityKey SecurityKey
        {
            get
            {
                var key = ECDsa.Create();
                key.ImportFromPem(PrivateKey);
                var secKey = new ECDsaSecurityKey(key);
                return secKey;
            }
        }

        public SigningCredentials Credentials =>
            new(SecurityKey, SecurityKey.KeySize switch
            {
                256 => SecurityAlgorithms.EcdsaSha256,
                384 => SecurityAlgorithms.EcdsaSha384,
                521 => SecurityAlgorithms.EcdsaSha512,
                _ => throw new NotSupportedException(),
            });
    }
}
