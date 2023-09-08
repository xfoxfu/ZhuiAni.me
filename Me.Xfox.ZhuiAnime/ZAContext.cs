using Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime;

public class ZAContext : DbContext
{
    public ZAContext(DbContextOptions<ZAContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Category { get; set; } = null!;
    public DbSet<Item> Item { get; set; } = null!;
    public DbSet<Link> Link { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;
    public DbSet<Session> Session { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            System.Reflection.Assembly.GetExecutingAssembly());
    }
}
