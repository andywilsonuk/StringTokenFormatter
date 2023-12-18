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
        var container = new SingleTokenValueContainer<int>(tokenName, value, settings);

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
        var container = new SingleTokenValueContainer<int>(tokenName, value, settings);

        var actual = container.TryMap("A");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenPolicyViolation_ReturnsDefault()
    {
        string tokenName = "a";
        string value = string.Empty;
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNullOrEmpty,
        };
        var container = new SingleTokenValueContainer<string>(tokenName, value, settings);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }
}