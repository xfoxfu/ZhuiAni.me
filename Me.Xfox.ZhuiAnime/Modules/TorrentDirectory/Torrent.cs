using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

public class Torrent
{
    public uint Id { get; set; }

    public string OriginSite { get; set; } = string.Empty;

    public string OriginId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Size { get; set; }

    [Column(TypeName = "jsonb")]
    public IDictionary<string, string>? Contents { get; set; }

    public DateTimeOffset PublishedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public JsonDocument OriginData { get; set; } = null!;

    public string? LinkTorrent { get; set; }

    public string? LinkMagnet { get; set; }

    public class TorrentConfiguration : IEntityTypeConfiguration<Torrent>
    {
        public void Configure(EntityTypeBuilder<Torrent> builder)
        {
            builder.Property(x => x.Size)
              .IsRequired(false);

            builder.Property(x => x.Contents)
              .IsRequired(false);

            builder.Property(x => x.LinkTorrent)
              .IsRequired(false);

            builder.Property(x => x.LinkMagnet)
              .IsRequired(false);

            builder.HasIndex(x => new { x.OriginSite, x.OriginId })
              .IsUnique();
        }
    }
}
