using System.Diagnostics.CodeAnalysis;
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

    public IEnumerable<Session> Sessions { get; set; } = null!;

    /// <summary>
    /// For protecting against timing side attacks. This is the hash of `HEE3KsHajmEq8bSX`.
    /// </summary>
    protected const string SomeInvalidPassword = "$2y$11$d3wI91tNqCkWwFiBUo6vq.UsmkxpTTHcoSvyMcTPPNvarDSNMcCyO";

    /// <summary>
    /// Validate the password of the user. This mitigates timing side attacks.
    /// After this function, the `user` will no longer be null.
    /// </summary>
    /// <param name="user">user or null</param>
    /// <param name="password">plain password</param>
    public static bool ValidatePassword([NotNullWhen(true)] User? user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user?.HashedPassword ?? SomeInvalidPassword);
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
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
