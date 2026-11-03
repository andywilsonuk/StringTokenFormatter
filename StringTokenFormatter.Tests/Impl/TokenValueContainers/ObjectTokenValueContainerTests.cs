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

    [Fact]
    public void TryMap_NestedProperty_WhenEnabled_ReturnsSuccess()
    {
        var source = new { Customer = new { Name = "Ada" } };
        StringTokenFormatterSettings settings = new()
        {
            ResolveNestedValues = NestedValueResolution.Nested,
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("Customer.Name");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "Ada" }, actual);
    }

    [Fact]
    public void TryMap_NestedProperty_WhenDisabled_ReturnsDefault()
    {
        var source = new { Customer = new { Name = "Ada" } };
        var container = TokenValueContainerFactory.FromObject(StringTokenFormatterSettings.Default, source);

        TryGetResult actual = container.TryMap("Customer.Name");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_NestedDictionary_WhenEnabled_ReturnsSuccess()
    {
        Dictionary<string, object> source = new()
        {
            ["Customer"] = new Dictionary<string, object> { ["Name"] = "Ada" },
        };
        StringTokenFormatterSettings settings = new() { ResolveNestedValues = NestedValueResolution.Nested };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("Customer.Name");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "Ada" }, actual);
    }

    [Fact]
    public void TryMap_NestedProperty_UsesConfiguredDelimiter()
    {
        var source = new { Customer = new { Name = "Ada" } };
        StringTokenFormatterSettings settings = new()
        {
            HierarchicalDelimiter = "/",
            ResolveNestedValues = NestedValueResolution.Nested,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("Customer/Name");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "Ada" }, actual);
    }

    [Fact]
    public void TryMap_NestedUnknownProperty_WhenEnabled_ReturnsDefault()
    {
        var source = new { Customer = new { Name = "Ada" } };
        StringTokenFormatterSettings settings = new() { ResolveNestedValues = NestedValueResolution.Nested };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        TryGetResult actual = container.TryMap("Customer.Bogus");

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TryMap_NestedAmbiguousPropertyMatch_WhenEnabled_Throws()
    {
        var source = new { Customer = new AmbiguousNames() };
        StringTokenFormatterSettings settings = new()
        {
            NameComparer = StringComparer.OrdinalIgnoreCase,
            ResolveNestedValues = NestedValueResolution.Nested,
        };
        var container = TokenValueContainerFactory.FromObject(settings, source);

        Assert.Throws<TokenContainerException>(() => container.TryMap("Customer.Name"));
    }

    private sealed class AmbiguousNames
    {
        public string Name => "lower";
        public string NAME => "upper";
    }
}