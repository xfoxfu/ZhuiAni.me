namespace Me.Xfox.ZhuiAnime.Modules.PikPak;

public class Anime
{
    public uint Id { get; set; }

    public string Source { get; set; } = string.Empty;

    public string Bangumi { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public string Regex { get; set; } = string.Empty;

    public MatchGroups MatchGroup { get; set; } = new MatchGroups();

    public DateTimeOffset LastFetchedAt { get; set; } = DateTimeOffset.MinValue;

    public class MatchGroups
    {
        /// <summary>
        /// `0` means disabled.
        /// </summary>
        /// <returns></returns>
        public uint Ep { get; set; }
    }
}
