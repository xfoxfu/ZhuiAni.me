using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Session
{
    public Ulid Token { get; set; } = Ulid.NewUlid();

    public User User { get; set; } = null!;
    public Ulid UserId { get; set; }

    public DateTimeOffset UserUpdatedAt { get; set; }

    public DateTimeOffset ExpiresIn { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.Token);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Sessions);
        }
    }
}
