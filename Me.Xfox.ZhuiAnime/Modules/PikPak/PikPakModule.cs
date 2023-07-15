namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class PikPakModule : IModule
{
    public static WebApplicationBuilder ConfigureOn(WebApplicationBuilder builder)
    {
        builder.Services.Configure<PikPakClient.Option>(builder.Configuration.GetSection(PikPakClient.Option.LOCATION));
        builder.Services.AddSingleton<PikPakClient>();
        return builder;
    }
}
