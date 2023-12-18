namespace StringTokenFormatter.Tests;

public class ObjectTokenValueContainerTests
{
    private record TestObject(int? A);

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
        var container = new ObjectTokenValueContainer<TestObject>(source, settings);

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
        var container = new ObjectTokenValueContainer<TestObject>(source, settings);

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
        var container = new ObjectTokenValueContainer<TestObject>(source, settings);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
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
        var container = new ObjectTokenValueContainer<TestObject>(source, settings);
        container.Frozen();

        var actual = container.TryMap("A");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

#endif
}