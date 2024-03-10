namespace StringTokenFormatter.Tests;

public class ObjectTokenValueContainerTests
{
    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        string value = "1";
        var source = new ComplexClass(value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        var actual = container.TryMap("value");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
        string value = "1";
        var source = new ComplexClass(value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        var actual = container.TryMap("value");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenPolicyViolation_ReturnsDefault()
    {
        string? value = null;
        var source = new ComplexClass(value!);
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

    [Fact]
    public void TryMap_Struct_ReturnsSuccess()
    {
        string value = "1";
        var source = new ComplexStruct(value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        var actual = container.TryMap("value");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void TryMap_FrozenDictionary_ReturnsSuccess()
    {
        string value = "1";
        var source = new ComplexClass(value);
        var settings = new StringTokenFormatterSettings
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);
        container.Frozen();

        var actual = container.TryMap("Value");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

#endif
}