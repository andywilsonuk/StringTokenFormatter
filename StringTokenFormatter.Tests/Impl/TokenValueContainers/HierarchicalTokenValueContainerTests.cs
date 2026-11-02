namespace StringTokenFormatter.Tests;

public class HierarchicalTokenValueContainerTests
{
    private const string prefix = "pre";
    private readonly BasicContainer innerContainer = new BasicContainer().Add("a", "1");

    [Fact]
    public void TryMap_TokenWithPrefix_ReturnsSuccess()
    {
        HierarchicalTokenValueContainer container = TokenValueContainerFactory.FromHierarchical(StringTokenFormatterSettings.Default, prefix, innerContainer);

        TryGetResult actual = container.TryMap($"{prefix}.a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }

    [Fact]
    public void TryMap_TokenWithoutPrefix_ReturnsFailure()
    {
        HierarchicalTokenValueContainer container = TokenValueContainerFactory.FromHierarchical(StringTokenFormatterSettings.Default, prefix, innerContainer);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = false, Value = default }, actual);
    }

    [Fact]
    public void TryMap_TokenWithIncorrectPrefix_ReturnsFailure()
    {
        HierarchicalTokenValueContainer container = TokenValueContainerFactory.FromHierarchical(StringTokenFormatterSettings.Default, prefix, innerContainer);

        TryGetResult actual = container.TryMap("bad.a");

        Assert.Equal(new TryGetResult { IsSuccess = false, Value = default }, actual);
    }

    [Fact]
    public void TryMap_TokenWithMissingSuffix_ReturnsFailure()
    {
        HierarchicalTokenValueContainer container = TokenValueContainerFactory.FromHierarchical(StringTokenFormatterSettings.Default, prefix, innerContainer);

        TryGetResult actual = container.TryMap($"{prefix}.");

        Assert.Equal(new TryGetResult { IsSuccess = false, Value = default }, actual);
    }

    [Fact]
    public void TryMap_TokenCasingComparerRespected_ReturnsSuccess()
    {
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        HierarchicalTokenValueContainer container = TokenValueContainerFactory.FromHierarchical(settings, prefix, innerContainer);

        TryGetResult actual = container.TryMap($"{prefix.ToUpperInvariant()}.a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }
}