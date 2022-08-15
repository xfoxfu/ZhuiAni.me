namespace Me.Xfox.ZhuiAnime.External.Bangumi.Test;

[TestClass]
public class BangumiClientTest
{
    [TestMethod]
    public async Task TestGetSubject()
    {
        var client = new BangumiClient();
        var subject = await client.GetSubjectAsync(364450);
        subject.Id.Should().Be(364450);
    }
}
