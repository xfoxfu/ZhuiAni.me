using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Models;
using Me.Xfox.ZhuiAnime.Models;

namespace Me.Xfox.ZhuiAnime.Services.Workflow;

public class CreateItemWithLink : CodeActivity
{
    public required Input<string> InTitle { get; set; }
    public required Input<Ulid> InCategoryId { get; set; }
    public Input<Ulid?>? InParentItemId { get; set; }
    public Input<IDictionary<string, string>>? InAnnotations { get; set; }
    public Input<string>? InImageUrl { get; set; }
    public required Input<Uri> InLinkAddress { get; set; }
    public required Input<string> InLinkMimeType { get; set; }

    public Output<Item>? OutItem { get; set; }
    public Output<Link>? OutLink { get; set; }

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var Db = context.GetRequiredService<ZAContext>();
        var Title = InTitle.Get(context);
        var CategoryId = InCategoryId.Get(context);
        var ParentItemId = InParentItemId.GetOrDefault(context);
        var Annotations = InAnnotations.GetOrDefault(context) ?? new Dictionary<string, string>();
        var ImageUrl = InImageUrl.GetOrDefault(context);
        var LinkAddress = InLinkAddress.Get(context);
        var LinkMimeType = InLinkMimeType.Get(context);

        using var tx = Db.Database.BeginTransaction();

        var item = await Db.Item.Where(a => a.Title == Title).FirstOrDefaultAsync()
            ?? await Db.Link.Where(l => l.Address == LinkAddress).Select(l => l.Item).FirstOrDefaultAsync();
        if (item == null)
        {
            item = new();
            Db.Item.Add(item);
        }

        item.Title = Title;
        item.CategoryId = CategoryId;
        item.ImageUrl = ImageUrl;
        item.Annotations = Annotations;
        item.ParentItemId = ParentItemId;

        await Db.SaveChangesAsync();

        var link = await Db.Link.Where(l => l.ItemId == item.Id && l.Address == LinkAddress)
            .FirstOrDefaultAsync();
        if (link == null)
        {
            link = new();
            Db.Link.Add(link);
        }
        link.ItemId = item.Id;
        link.Address = LinkAddress;
        link.MimeType = "text/html;kind=bgm.tv";

        await Db.SaveChangesAsync();
        await tx.CommitAsync();

        OutItem.Set(context, item);
        OutLink.Set(context, link);
    }
}
