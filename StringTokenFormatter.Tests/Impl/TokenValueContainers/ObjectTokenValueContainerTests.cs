namespace StringTokenFormatter.Tests;

public class ObjectTokenValueContainerTests
{
    private class TestObject
    {
        public TestObject(int? A) => this.A = A;
        public int? A { get; }
    }

    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        int value = 1;
        var source = new TestObject(A: value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
        int value = 1;
        var source = new TestObject(A: value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenPolicyViolation_ReturnsDefault()
    {
        int? value = null;
        var source = new TestObject(A: value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void Constructor_EmptySource_Throws()
    {
        var source = new {};
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromObject(settings, source));
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void TryMap_FrozenDictionary_ReturnsSuccess()
    {
        int value = 1;
        var source = new TestObject(A: value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);
        container.Frozen();

        var actual = container.TryMap("A");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

#endif
}