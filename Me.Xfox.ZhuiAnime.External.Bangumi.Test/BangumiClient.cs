namespace Me.Xfox.ZhuiAnime.External.Bangumi.Test;

[TestClass]
public class BangumiClientTest
{
    [TestMethod]
    public async Task TestGetSubject()
    {
        var client = new BangumiApi();
        var subject = await client.GetSubjectAsync(364450);
        subject.Id.Should().Be(364450);
    }

    [TestMethod]
    public async Task TestGetEpisodes()
    {
        var client = new BangumiApi();
        var episodes = await client.GetEpisodesAsync(296870).ToListAsync();
        episodes.Count.Should().BeGreaterThanOrEqualTo(13);
    }

    [TestMethod]
    public async Task TestGetEpisodesPaginated()
    {
        var client = new BangumiApi();
        var episodes = await client.GetEpisodesAsync(899).ToListAsync();
        episodes.Count.Should().BeGreaterThan(200);
    }

    [TestMethod]
    public async Task TestErrorHandling()
    {
        var client = new BangumiApi();
        var op = async () => await client.GetSubjectAsync(53397657);
        await op.Should().ThrowAsync<BangumiException>()
            .Where(e => e.Error != null
            && e.Error.Title == "Not Found"
            && e.Error.Description == "resource can't be found in the database or has been removed");

        var o2 = async () => await client.GetSubjectAsync(-1);
        await o2.Should().ThrowAsync<BangumiException>()
            .Where(e => e.Error != null
            && e.Error.Title == "Bad Request"
            && e.Error.Description.Contains("\"-1\" is not valid"));
    }

    [TestMethod]
    public async Task TestErrorHandlingNetworkError()
    {
        var client = new BangumiApi("https://localhost:53");
        var op = async () => await client.GetSubjectAsync(53397657);
        await op.Should().ThrowAsync<BangumiException>()
            .Where(e => e.Error == null && e.StatusCode == 0 && e.IsNetworkError);
    }
}
