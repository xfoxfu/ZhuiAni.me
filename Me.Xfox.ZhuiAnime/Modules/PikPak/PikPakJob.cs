using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class PikPakJob
{
    public Ulid IdV2 { get; set; }

    public uint Bangumi { get; set; }

    public string Target { get; set; } = string.Empty;

    public string Regex { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public MatchGroups MatchGroup { get; set; } = new MatchGroups();

    public DateTimeOffset LastFetchedAt { get; set; } = DateTimeOffset.MinValue;

    public bool Enabled { get; set; } = true;

    public class MatchGroups
    {
        /// <summary>
        /// `0` means disabled.
        /// </summary>
        /// <returns></returns>
        public uint Ep { get; set; }
    }

    public class AnimeConfiguration : IEntityTypeConfiguration<PikPakJob>
    {
        public void Configure(EntityTypeBuilder<PikPakJob> builder)
        {
            builder.ToTable("pikpak_job");

            builder.HasKey(x => x.IdV2);

            builder.OwnsOne(a => a.MatchGroup);
        }
    }
}
