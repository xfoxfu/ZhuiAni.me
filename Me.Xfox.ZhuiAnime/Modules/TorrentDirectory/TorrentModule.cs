namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

public class TorrentModule : IModule
{
    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        Worker<Sources.AcgRipSource>.ConfigureOn(builder);
        Worker<Sources.BangumiSource>.ConfigureOn(builder);
        return builder;
    }
}
