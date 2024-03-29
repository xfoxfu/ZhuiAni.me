using System.Text.Json;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public class LegacySubjectSmallTest
{
    [Test]
    public void TestDeserialize()
    {
        var result = JsonSerializer.Deserialize<LegacySubjectSmallTest>("""
{
    "id": 12,
    "url": "https://bgm.tv/subject/12",
    "type": 2,
    "name": "ちょびっツ",
    "name_cn": "人形电脑天使心",
    "summary": "在不久的将来,电子技术飞速发展,电脑成为人们生活中不可缺少的一部分.主角的名字是本须和秀树,是个19岁的少年,由于考试失败,来到东京上补习班,过着贫穷潦倒的生活……\\r\\n到达东京的第一天,他很幸运的在垃圾堆捡到一个人型电脑,一直以来秀树都非常渴望拥有个人电脑.当他抱着她带返公寓后,却不知如何开机,在意想不到的地方找到开关并开启后,故事就此展开\\r\\n本须和秀树捡到了人型计算机〔唧〕。虽然不晓得她到底是不是〔Chobits〕，但她的身上似乎藏有极大的秘密。看到秀树为了钱而烦恼，唧出去找打工，没想到却找到了危险的工作！为了让秀树开心，唧开始到色情小屋打工。但她在遭到过度激烈的强迫要求之后失控。让周遭计算机因此而强制停摆。\\r\\n另一方面，秀树发现好友新保与补习班的清水老师有着不可告人的关系……",
    "air_date": "2002-04-02",
    "air_weekday": 2,
    "images": {
        "large": "https://lain.bgm.tv/pic/cover/l/c2/0a/12_24O6L.jpg",
        "common": "https://lain.bgm.tv/pic/cover/c/c2/0a/12_24O6L.jpg",
        "medium": "https://lain.bgm.tv/pic/cover/m/c2/0a/12_24O6L.jpg",
        "small": "https://lain.bgm.tv/pic/cover/s/c2/0a/12_24O6L.jpg",
        "grid": "https://lain.bgm.tv/pic/cover/g/c2/0a/12_24O6L.jpg"
    },
    "eps": 27,
    "eps_count": 27,
    "rating": {
        "total": 2289,
        "count": {
            "1": 5,
            "2": 3,
            "3": 4,
            "4": 6,
            "5": 46,
            "6": 267,
            "7": 659,
            "8": 885,
            "9": 284,
            "10": 130
        },
        "score": 7.6
    },
    "rank": 573,
    "collection": {
        "wish": 608,
        "collect": 3010,
        "doing": 103,
        "on_hold": 284,
        "dropped": 86
    }
}
""");
    }
}
