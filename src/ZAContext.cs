using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime;

public class ZAContext : DbContext
{
    public ZAContext(DbContextOptions<ZAContext> options)
        : base(options)
    {
    }

    public DbSet<Anime> Animes { get; set; } = null!;
}
