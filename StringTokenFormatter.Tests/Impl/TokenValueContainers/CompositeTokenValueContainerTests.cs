namespace StringTokenFormatter.Tests;

public class CompositeTokenValueContainerTests
{
    private readonly BasicContainer innerContainer1 = new BasicContainer().Add("a", "1");
    private readonly BasicContainer innerContainer2 = new BasicContainer().Add("b", "2");

    [Fact]
    public void TryMap_TokenInFirst_ReturnsSuccess()
    {
        var container = TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default, innerContainer1, innerContainer2);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }

    [Fact]
    public void TryMap_TokenInSecond_ReturnsSuccess()
    {
        var container = TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default, innerContainer1, innerContainer2);

        var actual = container.TryMap("b");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "2" }, actual);
    }

    [Fact]
    public void Constructor_NullContainer_Throw()
    {
        Assert.Throws<ArgumentException>(() => TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default, innerContainer1, null!));
    }

    [Theory]
    [InlineData(TokenResolutionPolicy.IgnoreNull, null)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, null)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "")]
    public void TryMap_RejectedValue_ContinuesToNextContainer(TokenResolutionPolicy policy, string? firstValue)
    {
        var settings = StringTokenFormatterSettings.Default with { TokenResolutionPolicy = policy };
        var first = new BasicContainer().Add("value", firstValue);
        var second = new BasicContainer().Add("value", "fallback");
        var container = TokenValueContainerFactory.FromCombination(settings, first, second);

        var actual = container.TryMap("value");

        Assert.True(actual.IsSuccess);
        Assert.Equal("fallback", actual.Value);
    }

    [Theory]
    [InlineData(TokenResolutionPolicy.IgnoreNull, null)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, null)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "")]
    public void TryMap_AllValuesRejected_ReturnsUnresolved(TokenResolutionPolicy policy, string? value)
    {
        var settings = StringTokenFormatterSettings.Default with { TokenResolutionPolicy = policy };
        var container = TokenValueContainerFactory.FromCombination(settings,
            new BasicContainer().Add("value", value), new BasicContainer().Add("other", "unrelated"));

        var actual = container.TryMap("value");

        Assert.False(actual.IsSuccess);
    }

    [Theory]
    [InlineData(TokenResolutionPolicy.ResolveAll, null)]
    [InlineData(TokenResolutionPolicy.ResolveAll, "")]
    [InlineData(TokenResolutionPolicy.IgnoreNull, "")]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "first")]
    public void TryMap_AcceptedValue_PreservesFirstMatch(TokenResolutionPolicy policy, string? firstValue)
    {
        var settings = StringTokenFormatterSettings.Default with { TokenResolutionPolicy = policy };
        var container = TokenValueContainerFactory.FromCombination(settings,
            new BasicContainer().Add("value", firstValue), new UnexpectedContainer());

        var actual = container.TryMap("value");

        Assert.True(actual.IsSuccess);
        Assert.Equal(firstValue, actual.Value);
    }

    [Fact]
    public void TryMap_NoMatchingToken_ReturnsUnresolved()
    {
        var container = TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default,
            innerContainer1, innerContainer2);

        Assert.False(container.TryMap("missing").IsSuccess);
    }

    private sealed class UnexpectedContainer : ITokenValueContainer
    {
        public TryGetResult TryMap(string token) => throw new InvalidOperationException("Later containers should not be evaluated after an accepted match.");
    }
}
