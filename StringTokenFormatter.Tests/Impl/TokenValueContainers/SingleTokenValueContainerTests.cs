namespace StringTokenFormatter.Tests;

public class SingleTokenValueContainerTests
{
    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        string tokenName = "a";
        int value = 1;
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromSingle(settings, tokenName, value);

        var actual = container.TryMap("A");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
        string tokenName = "a";
        int value = 1;
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromSingle(settings, tokenName, value);

        var actual = container.TryMap("A");

        Assert.Equal(default, actual);
    }
}