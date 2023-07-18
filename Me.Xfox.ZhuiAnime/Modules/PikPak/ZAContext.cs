namespace Me.Xfox.ZhuiAnime;

public partial class ZAContext : DbContext
{
    public DbSet<Modules.PikPak.PikPakJob> PikPakJob { get; set; } = null!;
}
