using Microsoft.EntityFrameworkCore;
using Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime;

public class ZAContext : DbContext
{
    public ZAContext(DbContextOptions<ZAContext> options)
        : base(options)
    {
    }

    public DbSet<Anime> Anime { get; set; } = null!;
    public DbSet<Episode> Episode { get; set; } = null!;
    public DbSet<AnimeLink> AnimeLink { get; set; } = null!;
    public DbSet<EpisodeLink> EpisodeLink { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            System.Reflection.Assembly.GetExecutingAssembly());
    }
}
