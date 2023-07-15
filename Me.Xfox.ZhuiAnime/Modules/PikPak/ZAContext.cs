namespace Me.Xfox.ZhuiAnime;

public partial class ZAContext : DbContext
{
    public DbSet<Modules.PikPak.Anime> PikPakAnime { get; set; } = null!;
}
