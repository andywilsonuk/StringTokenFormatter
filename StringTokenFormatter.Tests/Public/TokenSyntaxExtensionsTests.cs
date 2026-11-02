namespace StringTokenFormatter.Tests;

public class TokenSyntaxExtensionsTests
{
    [Fact]
    public void Tokenize_SimpleTokenName_ReturnsWrappedToken()
    {
        string tokenName = "abc";
        TokenSyntax syntax = CommonTokenSyntax.Curly;

        string actual = syntax.Tokenize(tokenName);

        Assert.Equal("{abc}", actual);
    }

    [Fact]
    public void Escape_TokenStartMarkers_ReturnsLiteralText()
    {
        string value = "literal {token} and {{value}";
        TokenSyntax syntax = CommonTokenSyntax.Curly;

        string escaped = syntax.Escape(value);
        InterpolatedString parsed = InterpolatedStringParser.Parse(escaped, new StringTokenFormatterSettings { Syntax = syntax });

        Assert.All(parsed.Segments, segment => Assert.IsType<InterpolatedStringLiteralSegment>(segment));
        Assert.Equal(value, string.Concat(parsed.Segments.Cast<InterpolatedStringLiteralSegment>().Select(segment => segment.Raw)));
    }
}