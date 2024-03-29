using System.Text.Json;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public class ItemTest
{
    [Test]
    public void TestDeserialize()
    {
        JsonSerializer.Deserialize<ICollection<Item>>("""
  [
    { "key": "简体中文名", "value": "鲁路修·兰佩路基" },
    {
      "key": "别名",
      "value":
        [
          { "v": "L.L." },
          { "v": "勒鲁什" },
          { "v": "鲁鲁修" },
          { "v": "ゼロ" },
          { "v": "Zero" },
          { "k": "英文名", "v": "Lelouch Lamperouge" },
          { "k": "第二中文名", "v": "鲁路修·冯·布里塔尼亚" },
          { "k": "英文名二", "v": "Lelouch Vie Britannia" },
          { "k": "日文名", "v": "ルルーシュ・ヴィ・ブリタニア" }
        ]
    },
    { "key": "性别", "value": "男" },
    { "key": "生日", "value": "12月5日" },
    { "key": "血型", "value": "A型" },
    { "key": "身高", "value": "178cm→181cm" },
    { "key": "体重", "value": "54kg" },
    { "key": "引用来源", "value": "Wikipedia" }
  ]
""")!
        .ToList().Should().BeEquivalentTo(new List<Item>
        {
            new Item("简体中文名", "鲁路修·兰佩路基" ),
            new Item("别名", new List<Item.KVItem>
            {
                    new Item.KVItem("L.L."),
                    new Item.KVItem("勒鲁什"),
                    new Item.KVItem("鲁鲁修"),
                    new Item.KVItem("ゼロ"),
                    new Item.KVItem("Zero"),
                    new Item.KVItem("英文名", "Lelouch Lamperouge"),
                    new Item.KVItem("第二中文名", "鲁路修·冯·布里塔尼亚"),
                    new Item.KVItem("英文名二", "Lelouch Vie Britannia"),
                    new Item.KVItem("日文名", "ルルーシュ・ヴィ・ブリタニア"),
            }),
            new Item("性别", "男"),
            new Item("生日", "12月5日"),
            new Item("血型", "A型"),
            new Item("身高", "178cm→181cm"),
            new Item("体重", "54kg"),
            new Item("引用来源", "Wikipedia"),
        });

        FluentActions.Invoking(() => JsonSerializer.Deserialize<Item>("""
        {
          "key": "简体中文名",
          "value": {}
        }
        """))
          .Should().Throw<JsonException>()
          .WithMessage("ItemValue should be string or array, found StartObject instead.");
    }
}
