using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Link
{
    public Ulid Id { get; set; }

    public Item Item { get; set; } = null!;
    public Ulid ItemId { get; set; }

    public Uri Address { get; set; } = new Uri("invalid://");

    public string MimeType { get; set; } = "application/octet-stream";

    public IDictionary<string, string> Annotations { get; set; } = new Dictionary<string, string>();

    public Link? ParentLink { get; set; } = null;
    public Ulid? ParentLinkId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Link>? ChildLinks { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public class LinkConfiguration : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.HasOne(e => e.Item)
                .WithMany(e => e.Links);

            builder.HasOne(e => e.ParentLink)
                .WithMany(e => e.ChildLinks)
                .IsRequired(false);

            builder.Property(e => e.Annotations)
                .HasColumnType("jsonb");

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        }
    }

    public static class CommonAnnotations
    {
        public const string PikPakTorrentAddress = "me.xfox.zhuianime.modules.pikpak.torrent_address";
        public const string PikPakFileId = "https://api-drive.mypikpak.com/drive/v1/files/:id";
    }
}
