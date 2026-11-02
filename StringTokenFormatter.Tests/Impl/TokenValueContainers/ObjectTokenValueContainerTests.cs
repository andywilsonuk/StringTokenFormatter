namespace StringTokenFormatter.Tests;

public class ObjectTokenValueContainerTests
{
    [Fact]
    public void TryMap_MatchingTokenCaseInsensitive_ReturnsSuccess()
    {
        string value = "1";
        ComplexClass source = new(value);
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        ObjectTokenValueContainer<ComplexClass> container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("value");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

    [Fact]
    public void TryMap_MismatchedTokenCasing_ReturnsDefault()
    {
        string value = "1";
        ComplexClass source = new(value);
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        ObjectTokenValueContainer<ComplexClass> container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("value");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_MatchingTokenPolicyViolation_ReturnsDefault()
    {
        string? value = null;
        ComplexClass source = new(value!);
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
        };
        ObjectTokenValueContainer<ComplexClass> container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void Constructor_EmptySource_Throws()
    {
        var source = new { };
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
        };

        Assert.Throws<TokenContainerException>(() => TokenValueContainerFactory.FromObject(settings, source));
    }

    [Fact]
    public void TryMap_Struct_ReturnsSuccess()
    {
        string value = "1";
        ComplexStruct source = new(value);
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        ObjectTokenValueContainer<ComplexStruct> container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("value");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }

    [Fact]
    public void TryMap_FrozenDictionary_ReturnsSuccess()
    {
        string value = "1";
        ComplexClass source = new(value);
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.Ordinal,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        ObjectTokenValueContainer<ComplexClass> container = TokenValueContainerFactory.FromObject(settings, source);
        container.Frozen();

        TryGetResult actual = container.TryMap("Value");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = value }, actual);
    }
}