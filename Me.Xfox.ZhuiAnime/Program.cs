using System.Text.Json.Serialization;
using Me.Xfox.ZhuiAnime;
using Me.Xfox.ZhuiAnime.Utils;
using Me.Xfox.ZhuiAnime.Utils.Toml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ReplaceJsonWithToml();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<ZAContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString(nameof(ZAContext));
    if (connectionString == null)
    {
        throw new System.Exception("Connection string for ZAContext cannot be null");
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
