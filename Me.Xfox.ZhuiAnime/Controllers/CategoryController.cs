using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItemDto = Me.Xfox.ZhuiAnime.Controllers.ItemController.ItemDto;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>
/// Get category.
/// </summary>
[ApiController, Route("api/categories")]
public class CategoryController : ControllerBase
{
    private ZAContext DbContext { get; init; }

    public CategoryController(ZAContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>
    /// Category information.
    /// </summary>
    /// <param name="Id">id</param>
    /// <param name="Title">user-friendly name</param>
    public record CategoryDto(
        uint Id,
        string Title
    )
    {
        public CategoryDto(Category category) : this(category.Id, category.Title)
        {
        }
    }

    /// <summary>
    /// Get all categories.
    /// </summary>
    /// <returns>List of categories.</returns>
    [HttpGet]
    public async Task<IEnumerable<CategoryDto>> ListAsync()
    {
        return await DbContext.Category
            .OrderBy(a => a.Id)
            .Select(c => new CategoryDto(c))
            .ToListAsync();
    }

    /// <param name="Title">user-friendly name</param>
    public record CreateOrUpdateCategoryDto(string Title);

    /// <summary>
    /// Create a new category.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdateCategoryDto category)
    {
        var categoryDb = new Category
        {
            Title = category.Title
        };
        await DbContext.Category.AddAsync(categoryDb);
        await DbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { category_id = categoryDb.Id }, new CategoryDto(categoryDb));
    }

    protected async Task<Category> LoadCategory(uint id)
    {
        var category = await DbContext.Category.FindAsync(id);
        if (category == null)
        {
            throw new ZhuiAnimeError.CategoryNotFound(id);
        }
        return category;
    }

    /// <summary>
    /// Get a category.
    /// </summary>
    /// <param name="id">category id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<CategoryDto> Get(uint id)
    {
        return new CategoryDto(await LoadCategory(id));
    }

    /// <summary>
    /// Update a category.
    /// </summary>
    /// <param name="id">category id</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{id}")]
    public async Task<CategoryDto> Update(uint id, [FromBody] CreateOrUpdateCategoryDto request)
    {
        var category = await LoadCategory(id);
        category.Title = request.Title;
        await DbContext.SaveChangesAsync();
        return new CategoryDto(category);
    }

    /// <summary>
    /// Delete a category.
    /// </summary>
    /// <param name="id">category id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<CategoryDto> Delete(uint id)
    {
        var category = await LoadCategory(id);
        DbContext.Remove(category);
        await DbContext.SaveChangesAsync();
        return new CategoryDto(category);
    }

    /// <summary>
    /// Get a category's items.
    /// </summary>
    /// <remarks>
    /// This API will only return those are top-level, i.e. do not have a parent
    /// item. The result will be ordered by id descendingly.
    /// </remarks>
    /// <param name="id">category id</param>
    /// <returns></returns>
    [HttpGet("{id}/items")]
    public async Task<IEnumerable<ItemDto>> GetItems(uint id)
    {
        var category = await LoadCategory(id);
        var items = await DbContext.Entry(category)
            .Collection(c => c.Items!)
            .Query()
            .Where(i => i.ParentItemId == null)
            .OrderByDescending(i => i.Id)
            .Select(i => new ItemDto(i))
            .ToListAsync();
        return items;
    }
}
