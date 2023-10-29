namespace StringTokenFormatter.Tests;

public class StringTokenFormatterSettingsTests
{
    [Fact]
    public void Global_DefinedDefault_ReturnsDefaultValues()
    {
        var actual = StringTokenFormatterSettings.Global;

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
                TokenValueConverters.FromPrimitives(),
            }
        };

        Assert.Single(actual.ValueConverters);
    }

    [Fact]
    public void Global_OverideGlobal_ReturnsDefaultValues()
    {
        var actual = StringTokenFormatterSettings.Global with {
            NameComparer = StringComparer.CurrentCulture,
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
            Syntax = CommonTokenSyntax.DollarCurly,
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
            ValueConverters = new List<TokenValueConverter>
            {
                TokenValueConverters.FromNull(),
                TokenValueConverters.FromPrimitives(),
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
}