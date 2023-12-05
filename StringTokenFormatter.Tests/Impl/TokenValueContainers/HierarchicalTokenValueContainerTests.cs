namespace StringTokenFormatter.Tests;

public class HierarchicalTokenValueContainerTests
{
    private const string prefix = "pre";
    private readonly BasicContainer innerContainer = new BasicContainer().Add("a", "1");

    [Fact]
    public void TryMap_TokenWithPrefix_ReturnsSuccess()
    {
        var container = new HierarchicalTokenValueContainer(prefix, innerContainer, StringTokenFormatterSettings.Default);

        var actual = container.TryMap($"{prefix}.a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }

    [Fact]
    public void TryMap_TokenWithoutPrefix_ReturnsFailure()
    {
        var container = new HierarchicalTokenValueContainer(prefix, innerContainer, StringTokenFormatterSettings.Default);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = false, Value = default }, actual);
    }

    [Fact]
    public void TryMap_TokenWithIncorrectPrefix_ReturnsFailure()
    {
        var container = new HierarchicalTokenValueContainer(prefix, innerContainer, StringTokenFormatterSettings.Default);

        var actual = container.TryMap("bad.a");

        Assert.Equal(new TryGetResult { IsSuccess = false, Value = default }, actual);
    }

    [Fact]
    public void TryMap_TokenWithMissingSuffix_ReturnsFailure()
    {
        var container = new HierarchicalTokenValueContainer(prefix, innerContainer, StringTokenFormatterSettings.Default);

        var actual = container.TryMap($"{prefix}.");

        Assert.Equal(new TryGetResult { IsSuccess = false, Value = default }, actual);
    }

    [Fact]
    public void TryMap_TokenCasingComparerRespected_ReturnsSuccess()
    {
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new HierarchicalTokenValueContainer(prefix, innerContainer, settings);

        var actual = container.TryMap($"{prefix.ToUpperInvariant()}.a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }
}