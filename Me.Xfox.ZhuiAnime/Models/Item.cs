using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Item
{
    public Ulid IdV2 { get; set; }

    public Category Category { get; set; } = null!;
    public Ulid CategoryIdV2 { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public IDictionary<string, string> Annotations = new Dictionary<string, string>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Link>? Links { get; set; }

    public Item? ParentItem { get; set; } = null;
    public Ulid? ParentItemIdV2 { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Item>? ChildItems { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(x => x.IdV2);

            builder.HasOne(e => e.ParentItem)
                .WithMany(e => e.ChildItems)
                .HasForeignKey(x => x.ParentItemIdV2)
                .IsRequired(false);

            builder.HasOne(e => e.Category)
                .WithMany(e => e.Items)
                .HasForeignKey(x => x.CategoryIdV2);

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
