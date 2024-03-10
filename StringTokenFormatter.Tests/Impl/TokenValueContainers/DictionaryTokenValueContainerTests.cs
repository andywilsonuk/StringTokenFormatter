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
        var container = TokenValueContainerFactory.FromTuples(settings, pairs);

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
        var container = TokenValueContainerFactory.FromTuples(settings, pairs);

        var actual = container.TryMap("A");

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
        var container = TokenValueContainerFactory.FromTuples(settings, pairs);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void Constructor_CaseInsensitiveWithPreviousEquivalent_Throws()
    {
         var pairs = new (string, int?)[]
        {
            ("a", 1),
            ("A", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromTuples(settings, pairs));
    }
    
    [Fact]
    public void Constructor_EmptyTokenName_Throws()
    {
        var pairs = new (string, int?)[]
        {
            (string.Empty, 1),
            ("b", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromTuples(settings, pairs));
    }

    [Fact]
    public void Constructor_EmptySource_Throws()
    {
        var pairs = Array.Empty<(string, int?)>();
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromTuples(settings, pairs));
    }

    [Fact]
    public void TryMap_UsingKeyValuePairs_ReturnsSuccess()
    {
        var pairs = new[]
        {
            new KeyValuePair<string, int>("a", 1),
            new KeyValuePair<string, int>("b", 2),
        };
        var settings = new StringTokenFormatterSettings
        {
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromPairs(settings, pairs);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
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
        var container = TokenValueContainerFactory.FromTuples(settings, pairs);
        container.Frozen();

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }
#endif
}