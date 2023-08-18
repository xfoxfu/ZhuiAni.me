using System.Text.Json;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public class EpisodeTest
{
    [Test]
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
        .Should().BeEquivalentTo(new Episode
        {
            AirDate = "2021-08-14",
            Name = "総集編",
            NameCn = "总集篇",
            Duration = "00:23:40",
            Description = "",
            Ep = 0,
            Sort = 6.5,
            Id = 1047618,
            CommentCount = 11,
            Type = Episode.EpisodeType.SP,
            DiscCount = 0,
            DurationSeconds = 1420
        });
        JsonSerializer.Deserialize<Episode>("""
        {
            "airdate": "2023-03-11",
            "name": "警察学校編  Wild Police Story  CASE. 降谷零",
            "name_cn": "警察学校篇Wild Police Story CASE.降谷零",
            "duration": "",
            "desc": "由于没有总集数统计为1076.5话",
            "ep": 1076.5,
            "sort": 1076.5,
            "id": 1168761,
            "subject_id": 899,
            "comment": 5,
            "type": 0,
            "disc": 0,
            "duration_seconds": 0
        }
        """)!
        .Should().BeEquivalentTo(new Episode()
        {
            AirDate = "2023-03-11",
            Name = "警察学校編  Wild Police Story  CASE. 降谷零",
            NameCn = "警察学校篇Wild Police Story CASE.降谷零",
            Duration = "",
            Description = "由于没有总集数统计为1076.5话",
            Ep = 1076.5,
            Sort = 1076.5,
            Id = 1168761,
            CommentCount = 5,
            Type = 0,
            DiscCount = 0,
            DurationSeconds = 0
        });
        JsonSerializer.Deserialize<Episode>("""
        {
            "airdate": "2023-03-11",
            "name": "警察学校編  Wild Police Story  CASE. 降谷零",
            "name_cn": "警察学校篇Wild Police Story CASE.降谷零",
            "duration": "",
            "desc": "由于没有总集数统计为1076.5话",
            "id": 1168761,
            "subject_id": 899,
            "comment": 5,
            "type": 0,
            "disc": 0,
            "duration_seconds": 0
        }
        """)!
        .Should().BeEquivalentTo(new Episode()
        {
            AirDate = "2023-03-11",
            Name = "警察学校編  Wild Police Story  CASE. 降谷零",
            NameCn = "警察学校篇Wild Police Story CASE.降谷零",
            Duration = "",
            Description = "由于没有总集数统计为1076.5话",
            Ep = null,
            Sort = null,
            Id = 1168761,
            CommentCount = 5,
            Type = 0,
            DiscCount = 0,
            DurationSeconds = 0
        });
        FluentActions.Invoking(() => JsonSerializer.Deserialize<Episode>("{}"))
            .Should().Throw<JsonException>();
    }
}
