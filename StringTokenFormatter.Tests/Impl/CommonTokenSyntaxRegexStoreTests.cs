#if NET8_0_OR_GREATER

using System.Text.RegularExpressions;

namespace StringTokenFormatter.Tests;

public partial class CommonTokenSyntaxRegexStoreTests
{
    [Fact]
    public void GetRegex_CurlySyntax_ReturnsGeneratedRegex()
    {
        var actual = CommonTokenSyntaxRegexStore.GetRegex(CommonTokenSyntax.Curly);

        Assert.NotNull(actual);
    }

    [Fact]
    public void GetRegex_RoundSyntax_ReturnsGeneratedRegex()
    {
        var actual = CommonTokenSyntaxRegexStore.GetRegex(CommonTokenSyntax.Round);

        Assert.NotNull(actual);
    }

    [Fact]
    public void GetRegex_DollarRoundSyntax_ReturnsGeneratedRegex()
    {
        var actual = CommonTokenSyntaxRegexStore.GetRegex(CommonTokenSyntax.DollarRound);

        Assert.NotNull(actual);
    }

    [Fact]
    public void GetRegex_DollarCurlySyntax_ReturnsGeneratedRegex()
    {
        var actual = CommonTokenSyntaxRegexStore.GetRegex(CommonTokenSyntax.DollarCurly);

        Assert.NotNull(actual);
    }

    [Fact]
    public void GetRegex_DollarRoundAlternativeSyntax_ReturnsGeneratedRegex()
    {
        var actual = CommonTokenSyntaxRegexStore.GetRegex(CommonTokenSyntax.DollarRoundAlternative);

        Assert.NotNull(actual);
    }

    [GeneratedRegex(@"(-\*)|(\*.*?\*\*)", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetStarRegex();

    [Fact]
    public void GetRegex_StarSyntax_ReturnsGeneratedRegex()
    {
        var customSyntax = new TokenSyntax("*", "**", "-*");
        CommonTokenSyntaxRegexStore.AddCustom(customSyntax, GetStarRegex());

        var actual = CommonTokenSyntaxRegexStore.GetRegex(customSyntax);

        Assert.NotNull(actual);
    }
}

#endif