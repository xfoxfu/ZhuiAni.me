namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

public class BangumiModule : IModule
{
    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<Client.BangumiApi>();
        builder.Services.AddScoped<BangumiService>();
        return builder;
    }
}
