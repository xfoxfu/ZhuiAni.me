using System;
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
/// Operate links under an item.
/// </summary>
[ApiController, Route("api/items/{item_id}/links")]
public class ItemLinkController : ControllerBase
{
    private ZAContext DbContext { get; init; }

    public ItemLinkController(ZAContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>
    /// Link.
    /// </summary>
    /// <param name="Id">id</param>
    /// <param name="ItemId">id of the item this link belongs to</param>
    /// <param name="Address">the url this link points to</param>
    /// <param name="MimeType">the MIME type of the target of this link</param>
    /// <param name="Annotations">extra information for this link</param>
    /// <param name="ParentLinkId">id of parent link, if exists</param>
    public record LinkDto(
        uint Id,
        uint ItemId,
        Uri Address,
        string MimeType,
        IDictionary<string, string> Annotations,
        uint? ParentLinkId
    )
    {
        public LinkDto(Link link) : this(
            link.Id,
            link.ItemId,
            link.Address,
            link.MimeType,
            link.Annotations,
            link.ParentLinkId)
        {
        }
    }

    /// <summary>
    /// Get all links.
    /// </summary>
    /// <returns>List of links.</returns>
    [HttpGet]
    public async Task<IEnumerable<LinkDto>> ListAsync(uint item_id)
    {
        var item = await DbContext.Item.FindAsync(item_id);
        if (item == null)
        {
            throw new ZhuiAnimeError.ItemNotFound(item_id);
        }
        return await DbContext.Link
            .Where(l => l.ItemId == item.Id)
            .OrderBy(l => l.Id)
            .Select(c => new LinkDto(c))
            .ToListAsync();
    }

    /// <summary>
    /// Information for creating a link.
    /// </summary>
    /// <param name="Address">the url this link points to</param>
    /// <param name="MimeType">the MIME type of the target of this link</param>
    /// <param name="Annotations">extra information for this link</param>
    /// <param name="ParentLinkId">id of parent link, if exists</param>
    public record CreateLinkDto(
        Uri Address,
        string MimeType,
        IDictionary<string, string> Annotations,
        uint? ParentLinkId
    );

    /// <summary>
    /// Create a new link.
    /// </summary>
    /// <param name="item_id"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(LinkDto), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateAsync(uint item_id, [FromBody] LinkDto link)
    {
        var item = await DbContext.Item.FindAsync(item_id);
        if (item == null)
        {
            throw new ZhuiAnimeError.ItemNotFound(item_id);
        }
        var newLink = new Link
        {
            ItemId = item.Id,
            Address = link.Address,
            MimeType = link.MimeType,
            Annotations = link.Annotations,
            ParentLinkId = link.ParentLinkId
        };
        await DbContext.Link.AddAsync(newLink);
        await DbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = newLink.Id }, new LinkDto(newLink));
    }

    /// <summary>
    /// Get a link.
    /// </summary>
    /// <param name="id">link id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<LinkDto> Get(uint id)
    {
        var link = await DbContext.Link.FindAsync(id);
        if (link == null)
        {
            throw new ZhuiAnimeError.LinkNotFound(id);
        }
        return new LinkDto(link);
    }

    /// <summary>
    /// Information for creating a link.
    /// </summary>
    /// <param name="Address">the url this link points to</param>
    /// <param name="MimeType">the MIME type of the target of this link</param>
    /// <param name="Annotations">extra information for this link</param>
    public record UpdateLinkDto(
        Uri Address,
        string MimeType,
        IDictionary<string, string> Annotations
    );

    /// <summary>
    /// Update a category.
    /// </summary>
    /// <param name="id">category id</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{id}")]
    public async Task<LinkDto> Update(uint id, [FromBody] UpdateLinkDto request)
    {
        var link = await DbContext.Link.FindAsync(id);
        if (link == null)
        {
            throw new ZhuiAnimeError.CategoryNotFound(id);
        }
        link.Address = request.Address;
        link.MimeType = request.MimeType;
        link.Annotations = request.Annotations;
        await DbContext.SaveChangesAsync();
        return new LinkDto(link);
    }

    /// <summary>
    /// Delete a link.
    /// </summary>
    /// <param name="id">link id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<LinkDto> Delete(uint id)
    {
        var link = await DbContext.Link.FindAsync(id);
        if (link == null)
        {
            throw new ZhuiAnimeError.LinkNotFound(id);
        }
        DbContext.Link.Remove(link);
        await DbContext.SaveChangesAsync();
        return new LinkDto(link);
    }
}
