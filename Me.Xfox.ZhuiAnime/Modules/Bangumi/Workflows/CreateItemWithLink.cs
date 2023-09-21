using Me.Xfox.ZhuiAnime.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class CreateItemWithLink : StepBodyAsync
{
    protected ZAContext Db { get; set; }

    public string Name { get; set; } = string.Empty;
    public Ulid CategoryId { get; set; }
    public Ulid? ParentItemId { get; set; }
    public IDictionary<string, string> Annotations = new Dictionary<string, string>();
    public string? ImageUrl { get; set; }
    public Uri LinkAddress { get; set; } = new("invalid://");
    public string LinkMimeType { get; set; } = string.Empty;

    public Item Item { get; protected set; } = null!;
    public Link Link { get; protected set; } = null!;

    public CreateItemWithLink(ZAContext db)
    {
        Db = db;
    }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        using var tx = Db.Database.BeginTransaction();

        var item = await Db.Item.Where(a => a.Title == Name).FirstOrDefaultAsync()
            ?? await Db.Link.Where(l => l.Address == LinkAddress).Select(l => l.Item).FirstOrDefaultAsync();
        if (item == null)
        {
            item = new();
            Db.Item.Add(item);
        }

        item.Title = Name;
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

        Item = item;
        Link = link;
        return ExecutionResult.Next();
    }
}
