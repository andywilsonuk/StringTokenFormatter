namespace StringTokenFormatter.Tests;

public class DictionaryTokenValueContainerTests
{
    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        var pairs = new (string, int?)[]
        {
            ("a", 1),
            ("b", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new DictionaryTokenValueContainer<int?>(pairs, settings);

        var actual = container.TryMap("A");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
         var pairs = new (string, int?)[]
        {
            ("a", 1),
            ("b", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new DictionaryTokenValueContainer<int?>(pairs, settings);

        var actual = container.TryMap("A");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenPolicyViolation_ReturnsDefault()
    {
         var pairs = new (string, int?)[]
        {
            ("a", null),
            ("b", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
        };
        var container = new DictionaryTokenValueContainer<int?>(pairs, settings);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenCaseSensitive_ReturnsSuccess()
    {
         var pairs = new (string, int?)[]
        {
            ("a", 1),
            ("A", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new DictionaryTokenValueContainer<int?>(pairs, settings);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void TryMap_CaseInsensitiveOverridesPreviousEquivalent_ReturnsSuccess()
    {
         var pairs = new (string, int?)[]
        {
            ("a", 1),
            ("A", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new DictionaryTokenValueContainer<int?>(pairs, settings);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 2 }, actual);
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void TryMap_FrozenDictionary_ReturnsSuccess()
    {
        var pairs = new (string, int?)[]
        {
            ("a", 1),
            ("b", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new DictionaryTokenValueContainer<int?>(pairs, settings);
        container.Frozen();

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

#endif
}