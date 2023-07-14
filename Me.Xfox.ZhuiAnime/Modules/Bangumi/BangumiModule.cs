namespace Me.Xfox.ZhuiAnime.Modules.Bangumi;

public static class BangumiModule
{
    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<Client.BangumiApi>();
        builder.Services.AddSingleton<BangumiService>();
        return builder;
    }
}
