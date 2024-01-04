using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Models;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class FormatEpisodeName : CodeActivity<string>
{
    // Inputs:
    public required Input<Episode> InEpisode { get; set; }
    public required Input<uint> InTotalEpisodes { get; set; }
    // Outputs:
    public Output<string>? OutFormattedName { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        var Episode = InEpisode.Get(context);
        var TotalEpisodes = InTotalEpisodes.Get(context);

        int episodeNameLength = Convert.ToInt32(Math.Ceiling(Math.Log10(TotalEpisodes + 1)));
        string episodeNameFormat = $"{new string('0', episodeNameLength)}.###";

        var name = (Episode.Sort ?? 0).ToString(episodeNameFormat);
        if (Episode.Type == Episode.EpisodeType.SP) name = $"SP{name}";

        OutFormattedName.Set(context, name);
    }
}
