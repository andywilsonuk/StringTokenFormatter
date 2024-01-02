namespace StringTokenFormatter.Tests;

public class StringTokenFormatterSettingsTests : IDisposable
{
    [Fact]
    public void Default_DefinedDefault_ReturnsDefaultValues()
    {
        var actual = StringTokenFormatterSettings.Default;

        Assert.Equal(StringComparer.OrdinalIgnoreCase, actual.NameComparer);
        Assert.Equal(TokenResolutionPolicy.ResolveAll, actual.TokenResolutionPolicy);
        Assert.Equal(CommonTokenSyntax.Curly, actual.Syntax);
        Assert.Equal(UnresolvedTokenBehavior.Throw, actual.UnresolvedTokenBehavior);
        Assert.Equal(8, actual.ValueConverters.Count);
        Assert.Equal(CultureInfo.CurrentUICulture, actual.FormatProvider);
    }

    [Fact]
    public void Constructor_CustomSettings_ReturnsCustomValues()
    {
        var actual = new StringTokenFormatterSettings 
        {
            ValueConverters = new List<TokenValueConverter>
            {
                TokenValueConverters.PrimitiveConverter(),
            }
        };

        Assert.Single(actual.ValueConverters);
    }

    [Fact]
    public void Global_OverrideGlobal_ReturnsDefaultValues()
    {
        var actual = StringTokenFormatterSettings.Global with {
            NameComparer = StringComparer.CurrentCulture,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
            Syntax = CommonTokenSyntax.DollarCurly,
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
            ValueConverters = new List<TokenValueConverter>
            {
                TokenValueConverters.NullConverter(),
                TokenValueConverters.PrimitiveConverter(),
            },
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
        var actual = StringTokenFormatterSettings.Global;
        var expected = StringTokenFormatterSettings.Default;

        Assert.Equal(expected, actual);
    }

     [Fact]
    public void Global_ModifiedGlobal_NotSameAsDefault()
    {
        var defaultSyntax = StringTokenFormatterSettings.Default.Syntax;

        StringTokenFormatterSettings.Global = StringTokenFormatterSettings.Default with {
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
