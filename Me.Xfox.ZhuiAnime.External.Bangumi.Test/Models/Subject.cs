using System.Text.Json;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Test.Models;

[TestClass]
public class SubjectTest
{
    [TestMethod]
    public void TestDeserialize()
    {
        JsonSerializer.Deserialize<Subject>(@"
{
    ""date"": ""2022-07-02"",
    ""platform"": ""TV"",
    ""images"": {
        ""small"": ""https://lain.bgm.tv/pic/cover/s/65/19/364450_xx2zx.jpg"",
        ""grid"": ""https://lain.bgm.tv/pic/cover/g/65/19/364450_xx2zx.jpg"",
        ""large"": ""https://lain.bgm.tv/pic/cover/l/65/19/364450_xx2zx.jpg"",
        ""medium"": ""https://lain.bgm.tv/pic/cover/m/65/19/364450_xx2zx.jpg"",
        ""common"": ""https://lain.bgm.tv/pic/cover/c/65/19/364450_xx2zx.jpg""
    },
    ""summary"": ""平稳的日子――其实暗藏着秘密防范犯罪的秘密组织――“DA（Direct Attack）”。作为特工的少女们“LYCORIS”。有着历代最强的LYCORIS之称的・锦木千束、优秀的LYCORIS・井之上泷奈，在咖啡厅“LycoReco”支部工作。这里接受的订单从订购咖啡和甜点，到照顾小孩、代购、面向外国人的日语老师等请放心交给“LycoReco”自由自在的乐天派、和平主义的千束与沉着冷静系、效率主义的泷奈二人跌宕起伏的混乱日常正式上演！"",
    ""name"": ""リコリス・リコイル"",
    ""name_cn"": ""莉可丽丝"",
    ""tags"": [
        {
            ""name"": ""A-1Pictures"",
            ""count"": 1228
        },
        {
            ""name"": ""原创"",
            ""count"": 1173
        },
        {
            ""name"": ""百合"",
            ""count"": 1078
        },
        {
            ""name"": ""2022年7月"",
            ""count"": 831
        },
        {
            ""name"": ""战斗"",
            ""count"": 549
        },
        {
            ""name"": ""TV"",
            ""count"": 525
        },
        {
            ""name"": ""足立慎吾"",
            ""count"": 329
        },
        {
            ""name"": ""日常"",
            ""count"": 313
        },
        {
            ""name"": ""2022"",
            ""count"": 304
        },
        {
            ""name"": ""安済知佳"",
            ""count"": 171
        },
        {
            ""name"": ""无法预测的命运之舞台"",
            ""count"": 125
        },
        {
            ""name"": ""若山詩音"",
            ""count"": 97
        },
        {
            ""name"": ""A-1_Pictures"",
            ""count"": 41
        },
        {
            ""name"": ""轻百合"",
            ""count"": 18
        },
        {
            ""name"": ""2022年"",
            ""count"": 15
        },
        {
            ""name"": ""日本动画"",
            ""count"": 14
        },
        {
            ""name"": ""枪战"",
            ""count"": 13
        },
        {
            ""name"": ""搞笑"",
            ""count"": 13
        },
        {
            ""name"": ""石蒜物语"",
            ""count"": 11
        },
        {
            ""name"": ""冒险"",
            ""count"": 10
        },
        {
            ""name"": ""JK"",
            ""count"": 8
        },
        {
            ""name"": ""枪械"",
            ""count"": 7
        },
        {
            ""name"": ""日本"",
            ""count"": 7
        },
        {
            ""name"": ""贴贴"",
            ""count"": 6
        },
        {
            ""name"": ""美少女"",
            ""count"": 6
        },
        {
            ""name"": ""奇幻"",
            ""count"": 6
        },
        {
            ""name"": ""季番"",
            ""count"": 5
        },
        {
            ""name"": ""lycoris"",
            ""count"": 5
        },
        {
            ""name"": ""TVA"",
            ""count"": 4
        },
        {
            ""name"": ""小清水亜美"",
            ""count"": 4
        }
    ],
    ""infobox"": [
        {
            ""key"": ""中文名"",
            ""value"": ""莉可丽丝""
        },
        {
            ""key"": ""别名"",
            ""value"": [
                {
                    ""v"": ""铳动彼岸花""
                },
                {
                    ""v"": ""彼岸花咖啡厅""
                },
                {
                    ""v"": ""彼岸花的后坐力""
                },
                {
                    ""v"": ""リコリス リコイル""
                },
                {
                    ""v"": ""Lycoris Recoil""
                }
            ]
        },
        {
            ""key"": ""话数"",
            ""value"": ""13""
        },
        {
            ""key"": ""放送开始"",
            ""value"": ""2022年7月2日""
        },
        {
            ""key"": ""放送星期"",
            ""value"": ""星期六""
        },
        {
            ""key"": ""官方网站"",
            ""value"": ""https://lycoris-recoil.com/""
        },
        {
            ""key"": ""播放电视台"",
            ""value"": ""TOKYO MX""
        },
        {
            ""key"": ""其他电视台"",
            ""value"": ""BS11 / 群馬テレビ / とちぎテレビ""
        },
        {
            ""key"": ""Copyright"",
            ""value"": ""©Spider Lily／アニプレックス・ABCアニメーション・BS11""
        },
        {
            ""key"": ""原作"",
            ""value"": ""Spider Lily""
        },
        {
            ""key"": ""导演"",
            ""value"": ""足立慎吾""
        },
        {
            ""key"": ""人物设定"",
            ""value"": ""いみぎむる，辅助人设：山本由美子""
        },
        {
            ""key"": ""原画"",
            ""value"": ""【主动画师：沢田犬二(澤田謙治)】""
        },
        {
            ""key"": ""美术监督"",
            ""value"": ""冈本穗高、池田真依子""
        },
        {
            ""key"": ""企画"",
            ""value"": ""岩上敦宏、西出将之、田﨑勝也""
        },
        {
            ""key"": ""总制片"",
            ""value"": ""三宅将典；制作统括：柏田真一郎、加藤淳""
        },
        {
            ""key"": ""制片人"",
            ""value"": ""神宮司学、吉田佳弘、大和田智之；动画制片人：中柄裕二""
        },
        {
            ""key"": ""脚本"",
            ""value"": ""足立慎吾(1-2)、枦山大(3,7)、神林裕介(4)、鹿間貴裕(5-6)""
        },
        {
            ""key"": ""分镜"",
            ""value"": ""足立慎吾(1,3)、三浦貴博(2)、丸山裕介(3)、竹内哲也(4)、鹿間貴裕(5-6)、柴田裕介(7)""
        },
        {
            ""key"": ""演出"",
            ""value"": ""柴田裕介(1,7)、尾之上知久(2)、丸山裕介(3)、竹内哲也(4)、関暁子(5)、佐久間貴史(6)""
        },
        {
            ""key"": ""总作画监督"",
            ""value"": ""山本由美子(1,6)、竹内由香里(2,5)、鈴木豪(3-4)、晶貴孝二(3-4,7)、森田莉奈(7)""
        },
        {
            ""key"": ""文芸協力"",
            ""value"": ""枦山大、神林裕介""
        }
    ],
    ""rating"": {
        ""rank"": 482,
        ""total"": 2833,
        ""count"": {
            ""1"": 32,
            ""2"": 5,
            ""3"": 17,
            ""4"": 22,
            ""5"": 69,
            ""6"": 199,
            ""7"": 611,
            ""8"": 1251,
            ""9"": 347,
            ""10"": 280
        },
        ""score"": 7.7
    },
    ""total_episodes"": 14,
    ""collection"": {
        ""on_hold"": 78,
        ""dropped"": 118,
        ""wish"": 1464,
        ""collect"": 283,
        ""doing"": 7670
    },
    ""id"": 364450,
    ""eps"": 13,
    ""volumes"": 0,
    ""locked"": false,
    ""nsfw"": false,
    ""type"": 2
}
        ").Should().BeEquivalentTo(new Subject
        (
            Id: 364450,
            Type: SubjectType.Anime,
            Name: "リコリス・リコイル",
            NameCn: "莉可丽丝",
            Summary: "平稳的日子――其实暗藏着秘密防范犯罪的秘密组织――“DA（Direct Attack）”。作为特工的少女们“LYCORIS”。有着历代最强的LYCORIS之称的・锦木千束、优秀的LYCORIS・井之上泷奈，在咖啡厅“LycoReco”支部工作。这里接受的订单从订购咖啡和甜点，到照顾小孩、代购、面向外国人的日语老师等请放心交给“LycoReco”自由自在的乐天派、和平主义的千束与沉着冷静系、效率主义的泷奈二人跌宕起伏的混乱日常正式上演！",
            Nsfw: false,
            Locked: false,
            Date: "2022-07-02",
            Platform: "TV",
            Images: new Subject.ImagesData
            (
                Small: "https://lain.bgm.tv/pic/cover/s/65/19/364450_xx2zx.jpg",
                Grid: "https://lain.bgm.tv/pic/cover/g/65/19/364450_xx2zx.jpg",
                Large: "https://lain.bgm.tv/pic/cover/l/65/19/364450_xx2zx.jpg",
                Medium: "https://lain.bgm.tv/pic/cover/m/65/19/364450_xx2zx.jpg",
                Common: "https://lain.bgm.tv/pic/cover/c/65/19/364450_xx2zx.jpg"
            ),
            Infobox: new List<Item>
            {
                new Item("中文名", new Item.StringItemValue("莉可丽丝" )),
                new Item("别名", new Item.KVListItemValue(new List<Item.KVItem>
                {
                        new Item.KVItem(Value:"铳动彼岸花" ),
                        new Item.KVItem(Value:"彼岸花咖啡厅" ),
                        new Item.KVItem(Value:"彼岸花的后坐力" ),
                        new Item.KVItem(Value:"リコリス リコイル" ),
                        new Item.KVItem(Value:"Lycoris Recoil" ),
                })),
                new Item("话数", new Item.StringItemValue("13")),
                new Item("放送开始", new Item.StringItemValue("2022年7月2日")),
                new Item("放送星期", new Item.StringItemValue("星期六")),
                new Item("官方网站", new Item.StringItemValue("https://lycoris-recoil.com/")),
                new Item("播放电视台", new Item.StringItemValue("TOKYO MX")),
                new Item("其他电视台", new Item.StringItemValue("BS11 / 群馬テレビ / とちぎテレビ")),
                new Item("Copyright", new Item.StringItemValue("©Spider Lily／アニプレックス・ABCアニメーション・BS11")),
                new Item("原作", new Item.StringItemValue("Spider Lily")),
                new Item("导演", new Item.StringItemValue("足立慎吾")),
                new Item("人物设定", new Item.StringItemValue("いみぎむる，辅助人设：山本由美子")),
                new Item("原画", new Item.StringItemValue("【主动画师：沢田犬二(澤田謙治)】")),
                new Item("美术监督", new Item.StringItemValue("冈本穗高、池田真依子")),
                new Item("企画", new Item.StringItemValue("岩上敦宏、西出将之、田﨑勝也")),
                new Item("总制片", new Item.StringItemValue("三宅将典；制作统括：柏田真一郎、加藤淳")),
                new Item("制片人", new Item.StringItemValue("神宮司学、吉田佳弘、大和田智之；动画制片人：中柄裕二")),
                new Item("脚本", new Item.StringItemValue("足立慎吾(1-2)、枦山大(3,7)、神林裕介(4)、鹿間貴裕(5-6)")),
                new Item("分镜", new Item.StringItemValue("足立慎吾(1,3)、三浦貴博(2)、丸山裕介(3)、竹内哲也(4)、鹿間貴裕(5-6)、柴田裕介(7)")),
                new Item("演出", new Item.StringItemValue("柴田裕介(1,7)、尾之上知久(2)、丸山裕介(3)、竹内哲也(4)、関暁子(5)、佐久間貴史(6)")),
                new Item("总作画监督", new Item.StringItemValue("山本由美子(1,6)、竹内由香里(2,5)、鈴木豪(3-4)、晶貴孝二(3-4,7)、森田莉奈(7)")),
                new Item("文芸協力", new Item.StringItemValue("枦山大、神林裕介"))
            },
            Volumes: 0,
            Eps: 13,
            TotalEpisodes: 14,
            Rating: new Subject.RatingData
            (
                Rank: 482,
                Total: 2833,
                Count: new Subject.RatingData.CountData
                (
                    Rate1Count: 32,
                    Rate2Count: 5,
                    Rate3Count: 17,
                    Rate4Count: 22,
                    Rate5Count: 69,
                    Rate6Count: 199,
                    Rate7Count: 611,
                    Rate8Count: 1251,
                    Rate9Count: 347,
                    Rate10Count: 280
                ),
                Score: 7.7
            ),
            Collection: new Subject.CollectionData
            (
                Wish: 1464,
                Collect: 283,
                Doing: 7670,
                OnHold: 78,
                Dropped: 118
            ),
            Tags: new List<Subject.TagData>
            {
                new Subject.TagData(Name: "A-1Pictures", Count: 1228 ),
                new Subject.TagData(Name: "原创", Count: 1173 ),
                new Subject.TagData(Name: "百合", Count: 1078 ),
                new Subject.TagData(Name: "2022年7月", Count: 831 ),
                new Subject.TagData(Name: "战斗", Count: 549 ),
                new Subject.TagData(Name: "TV", Count: 525 ),
                new Subject.TagData(Name: "足立慎吾", Count: 329 ),
                new Subject.TagData(Name: "日常", Count: 313 ),
                new Subject.TagData(Name: "2022", Count: 304 ),
                new Subject.TagData(Name: "安済知佳", Count: 171 ),
                new Subject.TagData(Name: "无法预测的命运之舞台", Count: 125 ),
                new Subject.TagData(Name: "若山詩音", Count: 97 ),
                new Subject.TagData(Name: "A-1_Pictures", Count: 41 ),
                new Subject.TagData(Name: "轻百合", Count: 18 ),
                new Subject.TagData(Name: "2022年", Count: 15 ),
                new Subject.TagData(Name: "日本动画", Count: 14 ),
                new Subject.TagData(Name: "枪战", Count: 13 ),
                new Subject.TagData(Name: "搞笑", Count: 13 ),
                new Subject.TagData(Name: "石蒜物语", Count: 11 ),
                new Subject.TagData(Name: "冒险", Count: 10 ),
                new Subject.TagData(Name: "JK", Count: 8 ),
                new Subject.TagData(Name: "枪械", Count: 7 ),
                new Subject.TagData(Name: "日本", Count: 7 ),
                new Subject.TagData(Name: "贴贴", Count: 6 ),
                new Subject.TagData(Name: "美少女", Count: 6 ),
                new Subject.TagData(Name: "奇幻", Count: 6 ),
                new Subject.TagData(Name: "季番", Count: 5 ),
                new Subject.TagData(Name: "lycoris", Count: 5 ),
                new Subject.TagData(Name: "TVA", Count: 4 ),
                new Subject.TagData(Name: "小清水亜美", Count: 4 ),
            }
        ));
    }
}
