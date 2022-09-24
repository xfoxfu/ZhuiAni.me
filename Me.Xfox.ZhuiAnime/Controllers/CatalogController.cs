using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Me.Xfox.ZhuiAnime.Controllers;

[ApiController, Route("api/catalogs")]
public class CatalogController : ControllerBase
{
    private ZAContext DbContext { get; init; }
    private readonly IEnumerable<EndpointDataSource> endpointSources;

    public CatalogController(ZAContext dbContext, IEnumerable<EndpointDataSource> endpointSources)
    {
        DbContext = dbContext;
        this.endpointSources = endpointSources;
    }

    public record CatalogDto(
        uint Id,
        string Title
    );

    /// <summary>
    /// List all catalogs.
    /// </summary>
    /// <returns>list of catalogs</returns>
    [HttpGet]
    public async Task<IEnumerable<CatalogDto>> ListAsync()
    {
        return await DbContext.Catalog.Select(c => new CatalogDto(
            c.Id,
            c.Title
        )).ToListAsync();
    }

    public record CreateCatalogDto(
        string Title
    );

    /// <summary>
    /// Create a catalog.
    /// </summary>
    /// <param name="create"></param>
    /// <returns>catalog created</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CatalogDto), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateAsync(CreateCatalogDto create)
    {
        var catalog = new Catalog()
        {
            Title = create.Title
        };
        DbContext.Catalog.Add(catalog);
        await DbContext.SaveChangesAsync();
        return CreatedAtRoute($"{nameof(CatalogController)}.{nameof(GetAsync)}", new { catalog = catalog.Id }, new CatalogDto(catalog.Id, catalog.Title));
    }

    /// <summary>
    /// Get a catalog.
    /// </summary>
    /// <param name="catalog">catalog id</param>
    /// <returns>catalog</returns>
    [HttpGet("{catalog}", Name = $"{nameof(CatalogController)}.{nameof(GetAsync)}")]
    public CatalogDto GetAsync(Catalog catalog)
    {
        return new CatalogDto(
            catalog.Id,
            catalog.Title
        );
    }
}
