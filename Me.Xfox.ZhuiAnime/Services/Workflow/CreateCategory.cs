using Me.Xfox.ZhuiAnime.Models;
using Elsa.Extensions;
using Elsa.Workflows.Models;
using Elsa.Workflows;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class CreateCategory : CodeActivity<Category>
{
    public required Input<string> InTitle { get; set; }

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var Db = context.GetRequiredService<ZAContext>();
        var name = InTitle.Get(context);

        using var tx = await Db.Database.BeginTransactionAsync();
        var anime = await Db.Category.Where(a => a.Title == name).FirstOrDefaultAsync();
        if (anime == null)
        {
            anime = new();
            Db.Category.Add(anime);
        }

        anime.Title = name;

        await Db.SaveChangesAsync();
        await tx.CommitAsync();

        context.SetResult(anime);
    }
}
