namespace StringTokenFormatter.Tests;

public class SingleTokenValueContainerTests
{
    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        string tokenName = "a";
        int value = 1;
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        SingleTokenValueContainer<int> container = TokenValueContainerFactory.FromSingle(settings, tokenName, value);

        TryGetResult actual = container.TryMap("A");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
        string tokenName = "a";
        int value = 1;
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        SingleTokenValueContainer<int> container = TokenValueContainerFactory.FromSingle(settings, tokenName, value);

        TryGetResult actual = container.TryMap("A");

        Assert.Equal(default, actual);
    }
}