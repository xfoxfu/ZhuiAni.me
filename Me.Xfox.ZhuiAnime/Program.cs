global using Microsoft.EntityFrameworkCore;
using System.CommandLine;
using System.Text.Json.Serialization;
using Me.Xfox.ZhuiAnime;
using Me.Xfox.ZhuiAnime.Modules;
using Me.Xfox.ZhuiAnime.Services;
using Me.Xfox.ZhuiAnime.Utils;
using Me.Xfox.ZhuiAnime.Utils.Toml;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using ZhuiAnime = Me.Xfox.ZhuiAnime;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

builder.Configuration.ReplaceJsonWithToml();
builder.Configuration.AddTomlFile("appsettings.Local.toml", true);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
);

builder.Services.AddSpaYarp(opts =>
{
    opts.ClientUrl = "http://localhost:3000";
    opts.LaunchCommand = "pnpm dev";
    opts.WorkingDirectory = Path.Join(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Me.Xfox.ZhuiAnime.WebUI".Replace('\\', Path.DirectorySeparatorChar));
    opts.MaxTimeoutInSeconds = 3;
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers(opts =>
    {
        opts.OutputFormatters.RemoveType<StringOutputFormatter>();
        opts.OutputFormatters.RemoveType<StreamOutputFormatter>();
        opts.Filters.Add<ZAError.ErrorExceptionFilter>();

        var jsonInputFormatter = opts.InputFormatters.OfType<SystemTextJsonInputFormatter>().First();
        jsonInputFormatter.SupportedMediaTypes.Remove("text/json");
        jsonInputFormatter.SupportedMediaTypes.Remove("application/*+json");

        var jsonOutputFormatter = opts.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().First();
        jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
        jsonOutputFormatter.SupportedMediaTypes.Remove("application/*+json");
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
            throw new ZAError.BadRequest(context.ModelState);
    });

builder.Services.AddDbContext<ZAContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString(nameof(ZAContext)) ??
        throw new Exception("Connection string for ZAContext cannot be null");
    opt.UseNpgsql(connectionString);
    opt.UseSnakeCaseNamingConvention();
});

builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ZhuiAni.me API",
        Description = """
        # Error Handling

        ZhuiAni.me returns normalized error responses. The response body is a JSON object with the following fields:

        | Field           | Type     | Description     |
        | --------------- | -------- | --------------- |
        | `error_code`    | `string` | Error code.     |
        | `message`       | `string` | Error message.  |
        | `connection_id` | `string` | Connection ID.     |
        | `request_id`    | `string` | Request ID. |

        It may contain additional fields depending on the error code.

        For details, see the examples on each API endpoint. The additional fields is denoted like `{field}` in the
        error message example.
        """,
    });
    opts.AddServer(new OpenApiServer { Url = "https://zhuiani.me", Description = "Public server" });
    opts.AddServer(new OpenApiServer { Url = "https://localhost:5001", Description = "Local development server" });
    opts.AddSecurityDefinition("oauth2_1p", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Password = new OpenApiOAuthFlow
            {
                Scopes = new Dictionary<string, string>
                {
                    { TokenService.JwtScopes.OAuth, "Generate other OAuth tokens." },
                },
                TokenUrl = new Uri("{{baseUrl}}/api/session", UriKind.Relative),
            }
        },
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2_1p" }
            },
            Array.Empty<string>()
        }
    });
    opts.IncludeXmlComments(Path.Combine(
        AppContext.BaseDirectory,
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    opts.SupportNonNullableReferenceTypes();
    opts.SchemaFilter<RequiredNotNullableSchemaFilter>();
    opts.OperationFilter<SecurityRequirementsOperationFilter>();
    opts.OperationFilter<ZAError.ErrorResponsesOperationFilter>();
});

foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IModule))))
{
    Log.Logger.Information("Loading module {@Module}", type.FullName);
    type.GetMethod("ConfigureOn", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        ?.Invoke(null, new object[] { builder });
}
TurnstileService.ConfigureOn(builder);
TokenService.ConfigureOn(builder);

// FIXME: This will raise "No XML encryptor configured. Key {GUID} may be
// persisted to storage in unencrypted form." on start, but I think it is ok
// as the JWT keys are managed manually.
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.SaveToken = true;
    var config = builder.Configuration.GetSection(TokenService.Option.LOCATION)
        .Get<TokenService.Option>();
    new TokenService.OptionConfigure().Configure(config!);
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config?.Issuer,
        ValidateIssuer = true,
        ValidateAudience = false,
        IssuerSigningKey = config?.SecurityKey,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
    };
    opts.Events = new AuthenticationEventHandler();
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(c => c.RouteTemplate = "/api/swagger/{documentName}/swagger.json");
    app.UseReDoc(c =>
    {
        c.RoutePrefix = "api/swagger";
        c.SpecUrl = "/api/swagger/v1/swagger.json";
    });
}
else
{
    app.UseHsts();
}
app.UseForwardedHeaders();

app.UseSentryTracing();
app.UseSerilogRequestLogging();

if (app.Environment.IsProduction()) app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

app.MapFallbackToController("/api/{**path}",
    nameof(ZhuiAnime.Controllers.InternalController.EndpointNotFound),
    nameof(ZhuiAnime.Controllers.InternalController).Replace("Controller", ""));

if (app.Environment.IsDevelopment()) app.UseSpaYarp();

app.MapFallbackToFile("index.html");

var rootCommand = new RootCommand("Start ZhuiAni.me");
rootCommand.SetHandler(async () =>
{
    await app.RunAsync();
});
rootCommand.TreatUnmatchedTokensAsErrors = false;

var migrateCommand = new Command("migrate", "Migrate database");
rootCommand.Add(migrateCommand);
migrateCommand.SetHandler(async () =>
{
    using var scope = app.Services.CreateScope();
    using var db = scope.ServiceProvider.GetRequiredService<ZAContext>();
    await db.Database.MigrateAsync();
});
var migrateUlidCommand = new Command("ulid", "migrate to ulid");
migrateCommand.AddCommand(migrateUlidCommand);
migrateUlidCommand.SetHandler(async () =>
{
    using var scope = app.Services.CreateScope();
    using var db = scope.ServiceProvider.GetRequiredService<ZAContext>();

    foreach (var x in await db.Set<ZhuiAnime.Models.Category>().ToListAsync())
    {
        x.Id = Ulid.NewUlid(x.CreatedAt);
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Models.Item>().Include(x => x.Category).ToListAsync())
    {
        x.Id = Ulid.NewUlid(x.CreatedAt);
        x.CategoryId = x.Category.Id;
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Models.Item>().Include(x => x.ParentItem).ToListAsync())
    {
        x.ParentItemId = x.ParentItem?.Id;
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Models.Link>().Include(x => x.Item).ToListAsync())
    {
        x.Id = Ulid.NewUlid(x.CreatedAt);
        x.ItemId = x.Item.Id;
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Models.Link>().Include(x => x.ParentLink).ToListAsync())
    {
        x.ParentLinkId = x.ParentLink?.Id;
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Models.User>().ToListAsync())
    {
        x.Id = Ulid.NewUlid(x.CreatedAt);
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Models.Session>().Include(x => x.User).ToListAsync())
    {
        x.UserId = x.User.Id;
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Modules.PikPak.PikPakJob>().ToListAsync())
    {
        x.Id = Ulid.NewUlid(x.LastFetchedAt);
    }
    await db.SaveChangesAsync();
    foreach (var x in await db.Set<ZhuiAnime.Modules.TorrentDirectory.Torrent>().ToListAsync())
    {
        x.Id = Ulid.NewUlid(x.PublishedAt);
    }
    await db.SaveChangesAsync();
});

await rootCommand.InvokeAsync(args);
