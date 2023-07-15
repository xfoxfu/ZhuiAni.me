using System.ComponentModel.DataAnnotations.Schema;

namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class Anime
{
    public uint Id { get; set; }

    public uint Bangumi { get; set; }

    public string Target { get; set; } = string.Empty;

    public string Regex { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public MatchGroups MatchGroup { get; set; } = new MatchGroups();

    public DateTimeOffset LastFetchedAt { get; set; } = DateTimeOffset.MinValue;

    public bool Enabled { get; set; } = true;

    public class MatchGroups
    {
        /// <summary>
        /// `0` means disabled.
        /// </summary>
        /// <returns></returns>
        public uint Ep { get; set; }
    }
}
