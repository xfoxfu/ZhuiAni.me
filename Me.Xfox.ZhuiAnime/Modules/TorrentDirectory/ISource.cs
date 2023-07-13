namespace Me.Xfox.ZhuiAnime.Modules.TorrentDirectory;

public interface ISource
{
    string Name { get; }
    string Url { get; }
    Task<IList<Torrent>> GetPageAsync(uint id);
}
