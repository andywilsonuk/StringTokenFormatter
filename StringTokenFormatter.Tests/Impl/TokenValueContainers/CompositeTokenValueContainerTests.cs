namespace StringTokenFormatter.Tests;

public class CompositeTokenValueContainerTests
{
    private readonly BasicContainer innerContainer1 = new BasicContainer().Add("a", "1");
    private readonly BasicContainer innerContainer2 = new BasicContainer().Add("b", "2");

    [Fact]
    public void TryMap_TokenInFirst_ReturnsSuccess()
    {
        var container = new CompositeTokenValueContainer(new[] { innerContainer1, innerContainer2 }, StringTokenFormatterSettings.Default);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }

    [Fact]
    public void TryMap_TokenInSecond_ReturnsSuccess()
    {
        var container = new CompositeTokenValueContainer(new[] { innerContainer1, innerContainer2 }, StringTokenFormatterSettings.Default);

        var actual = container.TryMap("b");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "2" }, actual);
    }
}