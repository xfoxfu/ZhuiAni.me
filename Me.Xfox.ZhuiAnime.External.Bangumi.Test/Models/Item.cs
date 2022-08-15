using Me.Xfox.ZhuiAnime.External.Bangumi.Models;
using System.Text.Json;
using FluentAssertions;

namespace Me.Xfox.ZhuiAnime.External.Bangumi.Test.Models;

[TestClass]
public class ItemTest
{
    [TestMethod]
    public void TestDeserialize()
    {
        JsonSerializer.Deserialize<ICollection<Item>>(@"
  [
    { ""key"": ""简体中文名"", ""value"": ""鲁路修·兰佩路基"" },
    {
      ""key"": ""别名"",
      ""value"":
        [
          { ""v"": ""L.L."" },
          { ""v"": ""勒鲁什"" },
          { ""v"": ""鲁鲁修"" },
          { ""v"": ""ゼロ"" },
          { ""v"": ""Zero"" },
          { ""k"": ""英文名"", ""v"": ""Lelouch Lamperouge"" },
          { ""k"": ""第二中文名"", ""v"": ""鲁路修·冯·布里塔尼亚"" },
          { ""k"": ""英文名二"", ""v"": ""Lelouch Vie Britannia"" },
          { ""k"": ""日文名"", ""v"": ""ルルーシュ・ヴィ・ブリタニア"" }
        ]
    },
    { ""key"": ""性别"", ""value"": ""男"" },
    { ""key"": ""生日"", ""value"": ""12月5日"" },
    { ""key"": ""血型"", ""value"": ""A型"" },
    { ""key"": ""身高"", ""value"": ""178cm→181cm"" },
    { ""key"": ""体重"", ""value"": ""54kg"" },
    { ""key"": ""引用来源"", ""value"": ""Wikipedia"" }
  ]
")!
        .ToList().Should().BeEquivalentTo(new List<Item>
        {
            new Item { Key = "简体中文名", Value = new Item.StringItemValue { Value = "鲁路修·兰佩路基" } },
            new Item { Key = "别名", Value = new Item.KVListItemValue {
                Value = new List<Item.KVItem> {
                new Item.KVItem { Value = "L.L." },
                new Item.KVItem { Value = "勒鲁什" },
                new Item.KVItem { Value = "鲁鲁修" },
                new Item.KVItem { Value = "ゼロ" },
                new Item.KVItem { Value = "Zero" },
                new Item.KVItem { Key = "英文名", Value = "Lelouch Lamperouge" },
                new Item.KVItem { Key = "第二中文名", Value = "鲁路修·冯·布里塔尼亚" },
                new Item.KVItem { Key = "英文名二", Value = "Lelouch Vie Britannia" },
                new Item.KVItem { Key = "日文名", Value = "ルルーシュ・ヴィ・ブリタニア" },
                }
            }},
            new Item { Key = "性别", Value = new Item.StringItemValue { Value = "男"} },
            new Item { Key = "生日", Value = new Item.StringItemValue { Value = "12月5日"} },
            new Item { Key = "血型", Value = new Item.StringItemValue { Value = "A型"} },
            new Item { Key = "身高", Value = new Item.StringItemValue { Value = "178cm→181cm"} },
            new Item { Key = "体重", Value = new Item.StringItemValue { Value = "54kg"} },
            new Item { Key = "引用来源", Value = new Item.StringItemValue { Value = "Wikipedia"} },
        });
    }
}
