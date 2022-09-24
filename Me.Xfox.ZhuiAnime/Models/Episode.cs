using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

[ModelBinder(typeof(EpisodeModelBinder))]
public class Episode
{
    public uint Id { get; set; }

    public Anime Anime { get; set; } = null!;

    public uint AnimeId { get; set; }

    /// <summary>
    /// Anime episode name, like 01, SP01. Can be used for sorting.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public Uri BangumiLink { get; set; } = new Uri("invalid://");

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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

    public class EpisodeModelBinder : ZAModelBinder<Episode>
    {
        private readonly ZAContext DbContext;

        public EpisodeModelBinder(ZAContext dbContext)
        {
            DbContext = dbContext;
        }

        protected override string ModelName => "Episode ID";

        protected override ZhuiAnimeError GetError(uint id) => new ZhuiAnimeError.EpisodeNotFound(id);

        protected override ValueTask<Episode?> GetValue(uint id) => DbContext.Episode.FindAsync(id);
    }
}
