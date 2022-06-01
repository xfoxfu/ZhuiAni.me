using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Anime
{
    public uint Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public Uri BangumiLink { get; set; } = new Uri("invalid://");
    public IEnumerable<Episode>? Episodes { get; set; }
    public IEnumerable<AnimeLink>? Links { get; set; }

    public class AnimeConfiguration : IEntityTypeConfiguration<Anime>
    {
        public void Configure(EntityTypeBuilder<Anime> builder)
        {
        }
    }
}
