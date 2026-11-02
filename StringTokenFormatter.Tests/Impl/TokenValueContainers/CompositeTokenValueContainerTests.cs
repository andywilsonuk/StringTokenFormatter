namespace StringTokenFormatter.Tests;

public class CompositeTokenValueContainerTests
{
    private readonly BasicContainer innerContainer1 = new BasicContainer().Add("a", "1");
    private readonly BasicContainer innerContainer2 = new BasicContainer().Add("b", "2");

    [Fact]
    public void TryMap_TokenInFirst_ReturnsSuccess()
    {
        CompositeTokenValueContainer container = TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default, innerContainer1, innerContainer2);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "1" }, actual);
    }

    [Fact]
    public void TryMap_TokenInSecond_ReturnsSuccess()
    {
        CompositeTokenValueContainer container = TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default, innerContainer1, innerContainer2);

        TryGetResult actual = container.TryMap("b");

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
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with { TokenResolutionPolicy = policy };
        BasicContainer first = new BasicContainer().Add("value", firstValue);
        BasicContainer second = new BasicContainer().Add("value", "fallback");
        CompositeTokenValueContainer container = TokenValueContainerFactory.FromCombination(settings, first, second);

        TryGetResult actual = container.TryMap("value");

        Assert.True(actual.IsSuccess);
        Assert.Equal("fallback", actual.Value);
    }

    [Theory]
    [InlineData(TokenResolutionPolicy.IgnoreNull, null)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, null)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "")]
    public void TryMap_AllValuesRejected_ReturnsUnresolved(TokenResolutionPolicy policy, string? value)
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with { TokenResolutionPolicy = policy };
        CompositeTokenValueContainer container = TokenValueContainerFactory.FromCombination(settings,
            new BasicContainer().Add("value", value), new BasicContainer().Add("other", "unrelated"));

        TryGetResult actual = container.TryMap("value");

        Assert.False(actual.IsSuccess);
    }

    [Theory]
    [InlineData(TokenResolutionPolicy.ResolveAll, null)]
    [InlineData(TokenResolutionPolicy.ResolveAll, "")]
    [InlineData(TokenResolutionPolicy.IgnoreNull, "")]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "first")]
    public void TryMap_AcceptedValue_PreservesFirstMatch(TokenResolutionPolicy policy, string? firstValue)
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with { TokenResolutionPolicy = policy };
        CompositeTokenValueContainer container = TokenValueContainerFactory.FromCombination(settings,
            new BasicContainer().Add("value", firstValue), new UnexpectedContainer());

        TryGetResult actual = container.TryMap("value");

        Assert.True(actual.IsSuccess);
        Assert.Equal(firstValue, actual.Value);
    }

    [Fact]
    public void TryMap_NoMatchingToken_ReturnsUnresolved()
    {
        CompositeTokenValueContainer container = TokenValueContainerFactory.FromCombination(StringTokenFormatterSettings.Default,
            innerContainer1, innerContainer2);

        Assert.False(container.TryMap("missing").IsSuccess);
    }

    private sealed class UnexpectedContainer : ITokenValueContainer
    {
        public TryGetResult TryMap(string token) => throw new InvalidOperationException("Later containers should not be evaluated after an accepted match.");
    }
}
