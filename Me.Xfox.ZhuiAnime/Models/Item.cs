using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Item
{
    public uint Id { get; set; }
    public Ulid? IdV2 { get; set; }

    public Category Category { get; set; } = null!;
    public uint CategoryId { get; set; }
    public Ulid? CategoryIdV2 { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public IDictionary<string, string> Annotations = new Dictionary<string, string>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Link>? Links { get; set; }

    public Item? ParentItem { get; set; } = null;
    public uint? ParentItemId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Item>? ChildItems { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasOne(e => e.ParentItem)
                .WithMany(e => e.ChildItems)
                .IsRequired(false);

            builder.HasOne(e => e.Category)
                .WithMany(e => e.Items);

            builder.Property(e => e.Annotations)
                .HasColumnType("jsonb");

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
