using Me.Xfox.ZhuiAnime.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class CreateCategory : StepBodyAsync
{
    protected ZAContext Db;

    public CreateCategory(ZAContext db)
    {
        Db = db;
    }

    public string Title { get; set; } = string.Empty;
    public Category Category { get; set; } = null!;

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        using var tx = await Db.Database.BeginTransactionAsync();
        var anime = await Db.Category.Where(a => a.Title == Title).FirstOrDefaultAsync();
        if (anime == null)
        {
            anime = new();
            Db.Category.Add(anime);
        }

        anime.Title = Title;

        await Db.SaveChangesAsync();
        await tx.CommitAsync();

        Category = anime;
        return ExecutionResult.Next();
    }
}
