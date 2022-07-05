using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Episode
{
    public uint Id { get; set; }
    public Anime Anime { get; set; } = null!;
    public uint AnimeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Uri BangumiLink { get; set; } = new Uri("invalid://");
    public IEnumerable<EpisodeLink>? Links { get; set; }

    public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.HasIndex(e => e.BangumiLink)
                .IsUnique();

            builder.HasOne(e => e.Anime)
                .WithMany(e => e.Episodes);
        }
    }
}
