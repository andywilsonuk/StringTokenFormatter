namespace StringTokenFormatter.Tests;

public class StringTokenFormatterSettingsTests : IDisposable
{
    [Fact]
    public void Default_DefinedDefault_ReturnsDefaultValues()
    {
        StringTokenFormatterSettings actual = StringTokenFormatterSettings.Default;

        Assert.Equal(StringComparer.OrdinalIgnoreCase, actual.NameComparer);
        Assert.Equal(TokenResolutionPolicy.ResolveAll, actual.TokenResolutionPolicy);
        Assert.Equal(CommonTokenSyntax.Curly, actual.Syntax);
        Assert.Equal(UnresolvedTokenBehavior.Throw, actual.UnresolvedTokenBehavior);
        Assert.Equal(10, actual.ValueConverters.Count);
        Assert.Equal(CultureInfo.CurrentUICulture, actual.FormatProvider);
    }

    [Fact]
    public void Constructor_CustomSettings_ReturnsCustomValues()
    {
        StringTokenFormatterSettings actual = new()
        {
            ValueConverters =
            [
                TokenValueConverterFactory.PrimitiveConverter(),
            ]
        };

        Assert.Single(actual.ValueConverters);
    }

    [Fact]
    public void Global_OverrideGlobal_ReturnsDefaultValues()
    {
        StringTokenFormatterSettings actual = StringTokenFormatterSettings.Global with
        {
            NameComparer = StringComparer.CurrentCulture,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
            Syntax = CommonTokenSyntax.DollarCurly,
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
            ValueConverters =
            [
                TokenValueConverterFactory.NullConverter(),
                TokenValueConverterFactory.PrimitiveConverter(),
            ],
            FormatProvider = CultureInfo.InvariantCulture,
        };

        Assert.Equal(StringComparer.CurrentCulture, actual.NameComparer);
        Assert.Equal(TokenResolutionPolicy.IgnoreNull, actual.TokenResolutionPolicy);
        Assert.Equal(CommonTokenSyntax.DollarCurly, actual.Syntax);
        Assert.Equal(UnresolvedTokenBehavior.LeaveUnresolved, actual.UnresolvedTokenBehavior);
        Assert.Equal(2, actual.ValueConverters.Count);
        Assert.Equal(CultureInfo.InvariantCulture, actual.FormatProvider);
    }

    [Fact]
    public void Global_ComparedWithDefault_Same()
    {
        StringTokenFormatterSettings actual = StringTokenFormatterSettings.Global;
        StringTokenFormatterSettings expected = StringTokenFormatterSettings.Default;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Global_ModifiedGlobal_NotSameAsDefault()
    {
        TokenSyntax defaultSyntax = StringTokenFormatterSettings.Default.Syntax;

        StringTokenFormatterSettings.Global = StringTokenFormatterSettings.Default with
        {
            Syntax = CommonTokenSyntax.DollarRound,
        };

        Assert.Equal(defaultSyntax, StringTokenFormatterSettings.Default.Syntax);
        Assert.Equal(CommonTokenSyntax.DollarRound, StringTokenFormatterSettings.Global.Syntax);
    }

    public void Dispose()
    {
        StringTokenFormatterSettings.Global = StringTokenFormatterSettings.Default;
    }
}
