using System;
using System.Text.Json.Serialization;
using Me.Xfox.ZhuiAnime;
using Me.Xfox.ZhuiAnime.Utils;
using Me.Xfox.ZhuiAnime.Utils.Toml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ReplaceJsonWithToml();
builder.Services
    .AddControllers(options =>
    {
        options.OutputFormatters.RemoveType<StringOutputFormatter>();
        options.OutputFormatters.RemoveType<StreamOutputFormatter>();
        options.Filters.Add<ZhuiAnimeError.ErrorExceptionFilter>();
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
            throw new ZhuiAnimeError.BadRequest(context.ModelState);
    });

builder.Services.AddDbContext<ZAContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString(nameof(ZAContext));
    if (connectionString == null)
    {
        throw new Exception("Connection string for ZAContext cannot be null");
    }
    opt.UseNpgsql(connectionString);
    opt.UseSnakeCaseNamingConvention();
});

Me.Xfox.ZhuiAnime.Services.BangumiData.Option.ConfigureOn(builder);
Me.Xfox.ZhuiAnime.Services.BangumiClient.Option.ConfigureOn(builder);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "ZhuiAni.me API",
    });
    options.OperationFilter<ZhuiAnimeError.ErrorResponsesOperationFilter>();
    options.SchemaFilter<RequiredNotNullableSchemaFilter>();
    options.SupportNonNullableReferenceTypes();
});

builder.Services.AddSingleton<Me.Xfox.ZhuiAnime.Services.BangumiClient>();

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

if (app.Environment.IsProduction()) app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}/{id?}");

app.MapFallbackToController("/api/{**path}",
    nameof(Me.Xfox.ZhuiAnime.Controllers.InternalController.EndpointNotFound),
    nameof(Me.Xfox.ZhuiAnime.Controllers.InternalController).Replace("Controller", ""));
app.MapFallbackToFile("index.html");

await app.RunAsync();
