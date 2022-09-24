using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Serilog;

namespace Me.Xfox.ZhuiAnime.Models;

[ModelBinder(typeof(AnimeModelBinder))]
public class Anime
{
    public uint Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public byte[]? Image { get; set; } = null;

    public string ImageBase64 { get => Convert.ToBase64String(Image ?? Array.Empty<byte>()); }

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

    public class AnimeModelBinder : ZAModelBinder<Anime>
    {
        private readonly ZAContext DbContext;

        public AnimeModelBinder(ZAContext dbContext, ILogger logger) : base(logger)
        {
            DbContext = dbContext;
        }

        protected override string ModelName => "Anime ID";

        protected override ZhuiAnimeError GetError(uint id) => new ZhuiAnimeError.AnimeNotFound(id);

        protected override Task<Anime?> GetValue(uint id) => DbContext.Anime.FindAsync(id).AsTask();
    }
}
