using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Me.Xfox.ZhuiAnime.Models;

public class Catalog
{
    public uint Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public IEnumerable<Link>? Links { get; set; }

    public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
        }
    }
}
