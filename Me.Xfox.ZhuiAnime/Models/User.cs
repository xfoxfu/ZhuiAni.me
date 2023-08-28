using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class User
{
    public uint Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string HashedPassword { get; protected set; } = string.Empty;

    public string Password { set => HashedPassword = BCrypt.Net.BCrypt.HashPassword(value); }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool ValidatePassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, HashedPassword);
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.Username)
                .IsUnique();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnUpdate();
        }
    }
}
