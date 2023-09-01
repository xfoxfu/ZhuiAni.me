using Me.Xfox.ZhuiAnime.Models;
using Microsoft.AspNetCore.Mvc;
using ItemDto = Me.Xfox.ZhuiAnime.Controllers.ItemController.ItemDto;

namespace Me.Xfox.ZhuiAnime.Controllers;

/// <summary>Get category.</summary>
[ApiController, Route("api/categories")]
public class CategoryController : ControllerBase
{
    private ZAContext DbContext { get; init; }

    public CategoryController(ZAContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>Category information.</summary>
    /// <param name="Id">id</param>
    /// <param name="Title">user-friendly name</param>
    /// <param name="CreatedAt">created time</param>
    /// <param name="UpdatedAt">last updated time</param>
    public record CategoryDto(
        uint Id,
        string Title,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    )
    {
        public CategoryDto(Category category) : this(
            category.Id,
            category.Title,
            category.CreatedAt,
            category.UpdatedAt)
        {
        }
    }

    /// <summary>List</summary>
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

    /// <summary>Create</summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
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

    protected async Task<Category> LoadCategory(string routeParam = "id")
    {
        Request.RouteValues.TryGetValue(routeParam, out var idStr);
        var id = Convert.ToUInt32(idStr ?? "0");
        var category = await DbContext.Category.FindAsync(id) ??
            throw new ZAError.CategoryNotFound(id);
        return category;
    }

    /// <summary>Get</summary>
    /// <param name="id">category id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ZAError.Has<ZAError.CategoryNotFound>]
    public async Task<CategoryDto> Get(uint id)
    {
        return new CategoryDto(await LoadCategory());
    }

    /// <summary>Update</summary>
    /// <param name="id">category id</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [ZAError.Has<ZAError.CategoryNotFound>]
    public async Task<CategoryDto> Update(uint id, [FromBody] CreateOrUpdateCategoryDto request)
    {
        var category = await LoadCategory();
        category.Title = request.Title;
        await DbContext.SaveChangesAsync();
        return new CategoryDto(category);
    }

    /// <summary>Delete</summary>
    /// <param name="id">category id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ZAError.Has<ZAError.CategoryNotFound>]
    public async Task<CategoryDto> Delete(uint id)
    {
        var category = await LoadCategory();
        DbContext.Remove(category);
        await DbContext.SaveChangesAsync();
        return new CategoryDto(category);
    }

    /// <summary>Get Child Items</summary>
    /// <remarks>
    /// This API will only return those are top-level, i.e. do not have a parent
    /// item. The result will be ordered by id descendingly.
    /// </remarks>
    /// <param name="id">category id</param>
    /// <returns></returns>
    [HttpGet("{id}/items")]
    [ZAError.Has<ZAError.CategoryNotFound>]
    public async Task<IEnumerable<ItemDto>> GetItems(uint id)
    {
        var category = await LoadCategory();
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
