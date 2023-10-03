namespace StringTokenFormatter.Tests;

public class InterpolatedStringResolverTests
{
    [Fact]
    public void FromSingle_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var resolver = new InterpolatedStringResolver(settings);

        string actual = resolver.FromSingle(source, "two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromSingle_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);

        string actual = resolver.FromSingle(interpolatedString, "two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
}