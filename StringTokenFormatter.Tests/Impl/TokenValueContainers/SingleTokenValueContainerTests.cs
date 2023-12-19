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
        var container = TokenValueContainerFactory.FromSingle(settings, tokenName, value);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void Constructor_EmptyTokenName_Throws()
    {
        string tokenName = string.Empty;
        int value = 1;
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };

        Assert.Throws<InvalidTokenNameException>(() => TokenValueContainerFactory.FromSingle(settings, tokenName, value));
    }
}