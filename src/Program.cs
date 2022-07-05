using Me.Xfox.ZhuiAnime;
using Me.Xfox.ZhuiAnime.Utils;
using Me.Xfox.ZhuiAnime.Utils.Toml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureAppConfiguration((ctx, builder) =>
{
    var json1 = builder.Sources.IndexOf(builder.Sources.FirstOrDefault(
        s => s is JsonConfigurationSource j && j.Path.Contains("appsettings.json")));
    builder.AddTomlFile("appsettings.toml", optional: true, index: json1);

    var json2 = builder.Sources.IndexOf(builder.Sources.FirstOrDefault(
        s => s is JsonConfigurationSource j && j.Path.Contains($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json")));
    builder.AddTomlFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.toml", optional: true, index: json2);
});

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
    });

builder.Services.AddDbContext<ZAContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ZAContext)));
    opt.UseSnakeCaseNamingConvention();
});

Me.Xfox.ZhuiAnime.Services.BangumiData.Option.ConfigureOn(builder);
Me.Xfox.ZhuiAnime.Services.BangumiClient.Option.ConfigureOn(builder);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ZhuiAni.me API", Version = "v1" });
});

builder.Services.AddHostedService<Me.Xfox.ZhuiAnime.Services.BangumiData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(c => c.RouteTemplate = "api/swagger/{documentName}/swagger.json");
    app.UseReDoc(c =>
    {
        c.SpecUrl("/api/swagger/v1/swagger.json");
        c.RoutePrefix = "api/swagger";
    });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

await app.RunAsync();
