using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class FormatEpisodeName : StepBody
{
    // Inputs:
    public Episode Episode { get; set; } = null!;
    public uint TotalEpisodes { get; set; }
    // Outputs:
    public string FormattedName { get; set; } = string.Empty;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        int episodeNameLength = Convert.ToInt32(Math.Ceiling(Math.Log10(TotalEpisodes + 1)));
        string episodeNameFormat = $"{new string('0', episodeNameLength)}.###";

        var name = (Episode.Sort ?? 0).ToString(episodeNameFormat);
        if (Episode.Type == Episode.EpisodeType.SP) name = $"SP{name}";

        FormattedName = name;
        return ExecutionResult.Next();
    }
}
