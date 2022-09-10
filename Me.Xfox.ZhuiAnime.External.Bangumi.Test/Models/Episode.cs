using System.Text.Json;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Test.Models;

[TestClass]
public class EpisodeTest
{
    [TestMethod]
    public void TestDeserialize()
    {
        JsonSerializer.Deserialize<Episode>("""
        {
            "airdate": "2021-08-14",
            "name": "総集編",
            "name_cn": "总集篇",
            "duration": "00:23:40",
            "desc": "",
            "ep": 0,
            "sort": 6.5,
            "id": 1047618,
            "subject_id": 296870,
            "comment": 11,
            "type": 1,
            "disc": 0,
            "duration_seconds": 1420
        }
        """)!
        .Should().BeEquivalentTo(new Episode(
            AirDate: "2021-08-14",
            Name: "総集編",
            NameCn: "总集篇",
            Duration: "00:23:40",
            Description: "",
            Ep: 0,
            Sort: 6.5,
            Id: 1047618,
            CommentCount: 11,
            Type: Episode.EpisodeType.SP,
            DiscCount: 0,
            DurationSeconds: 1420
        ));
    }
}
