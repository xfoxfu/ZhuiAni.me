using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Serilog;

namespace Me.Xfox.ZhuiAnime.Models;

[ModelBinder(typeof(CatalogModelBinder))]
public class Catalog
{
    public uint Id { get; set; }

    public string Title { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Link>? Links { get; set; }

    public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
        }
    }


    public class CatalogModelBinder : ZAModelBinder<Catalog>
    {
        private readonly ZAContext DbContext;

        public CatalogModelBinder(ZAContext dbContext, ILogger logger) : base(logger)
        {
            DbContext = dbContext;
        }

        protected override string ModelName => "Catalog ID";

        protected override ZhuiAnimeError GetError(uint id) => new ZhuiAnimeError.CatalogNotFound(id);

        protected override Task<Catalog?> GetValue(uint id) => DbContext.Catalog.FindAsync(id).AsTask();
    }
}
