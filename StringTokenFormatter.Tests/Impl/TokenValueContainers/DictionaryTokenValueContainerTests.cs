using System.Collections.Frozen;

namespace StringTokenFormatter.Tests;

public class DictionaryTokenValueContainerTests
{
    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        (string, int?)[] pairs =
        [
            ("a", 1),
            ("b", 2),
        ];
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        DictionaryTokenValueContainer<int?> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("A");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
        (string, int?)[] pairs =
       [
            ("a", 1),
            ("b", 2),
       ];
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        DictionaryTokenValueContainer<int?> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("A");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenCaseSensitive_ReturnsSuccess()
    {
        (string, int?)[] pairs =
       [
            ("a", 1),
            ("A", 2),
       ];
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        DictionaryTokenValueContainer<int?> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void Constructor_CaseInsensitiveWithPreviousEquivalent_Throws()
    {
        (string, int?)[] pairs =
       [
            ("a", 1),
            ("A", 2),
       ];
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromTuples(settings, pairs));
    }

    [Fact]
    public void Constructor_EmptyTokenName_Throws()
    {
        (string, int?)[] pairs =
        [
            (string.Empty, 1),
            ("b", 2),
        ];
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromTuples(settings, pairs));
    }

    [Fact]
    public void Constructor_EmptySource_Throws()
    {
        (string, int?)[] pairs = Array.Empty<(string, int?)>();
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromTuples(settings, pairs));
    }

    [Fact]
    public void TryMap_UsingKeyValuePairs_ReturnsSuccess()
    {
        KeyValuePair<string, int>[] pairs =
        [
            new KeyValuePair<string, int>("a", 1),
            new KeyValuePair<string, int>("b", 2),
        ];
        StringTokenFormatterSettings settings = new()
        {
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        DictionaryTokenValueContainer<int> container = TokenValueContainerFactory.FromPairs(settings, pairs);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }


    [Fact]
    public void TryMap_FrozenDictionary_ReturnsSuccess()
    {
        (string, int?)[] pairs =
        [
            ("a", 1),
            ("b", 2),
        ];
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        DictionaryTokenValueContainer<int?> container = TokenValueContainerFactory.FromTuples(settings, pairs);
        container.Frozen();

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void TryMap_NestedObjectValue_WhenEnabled_ReturnsSuccess()
    {
        (string, object)[] pairs = [("Customer", new { Name = "Ada" })];
        StringTokenFormatterSettings settings = new() { ResolveNestedValues = NestedValueResolution.Nested };
        DictionaryTokenValueContainer<object> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("Customer.Name");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "Ada" }, actual);
    }

    [Fact]
    public void TryMap_NestedDictionaryValue_WhenEnabled_ReturnsSuccess()
    {
        (string, object)[] pairs =
        [
            ("Customer", new Dictionary<string, object> { ["Name"] = "Ada" }),
        ];
        StringTokenFormatterSettings settings = new() { ResolveNestedValues = NestedValueResolution.Nested };
        DictionaryTokenValueContainer<object> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("Customer.Name");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "Ada" }, actual);
    }

    [Fact]
    public void TryMap_NestedFrozenDictionaryValue_WhenEnabled_ReturnsSuccess()
    {
        (string, object)[] pairs =
        [
            ("Customer", new Dictionary<string, object> { ["Name"] = "Ada" }.ToFrozenDictionary()),
        ];
        StringTokenFormatterSettings settings = new() { ResolveNestedValues = NestedValueResolution.Nested };
        DictionaryTokenValueContainer<object> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("Customer.Name");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "Ada" }, actual);
    }

    [Fact]
    public void TryMap_NestedUnknownChildValue_WhenEnabled_ReturnsDefault()
    {
        (string, object)[] pairs = [("Customer", new { Name = "Ada" })];
        StringTokenFormatterSettings settings = new() { ResolveNestedValues = NestedValueResolution.Nested };
        DictionaryTokenValueContainer<object> container = TokenValueContainerFactory.FromTuples(settings, pairs);

        TryGetResult actual = container.TryMap("Customer.Bogus");

        Assert.Equal(default, actual);
    }
}