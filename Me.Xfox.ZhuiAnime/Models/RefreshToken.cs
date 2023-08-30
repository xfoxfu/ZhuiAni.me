using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class RefreshToken
{
    public Guid Token { get; set; }

    public User User { get; set; } = null!;
    public uint UserId { get; set; }

    public DateTimeOffset UserUpdatedAt { get; set; }

    public DateTimeOffset ExpiresIn { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Token);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens);
        }
    }
}
