using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Anime
{
    public uint Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public byte[]? Image { get; set; } = null;

    public Uri BangumiLink { get; set; } = new Uri("invalid://");

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Episode>? Episodes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<AnimeLink>? Links { get; set; }

    public class AnimeConfiguration : IEntityTypeConfiguration<Anime>
    {
        public void Configure(EntityTypeBuilder<Anime> builder)
        {
            builder.HasIndex(a => a.BangumiLink)
                .IsUnique();
        }
    }
}
