namespace StringTokenFormatter.Tests;

public class InterpolatedStringParserTests
{
    [Fact]
    public void Parse_WithToken_ReturnsInterpolatedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringSegment("first "), a),
            a => Assert.Equal(new InterpolatedStringTokenSegment("(two)", "two", string.Empty, string.Empty), a),
            a => Assert.Equal(new InterpolatedStringSegment(" third"), a)
        );
        Assert.Equal(settings, actual.Settings);
    }

    [Fact]
    public void Parse_WithoutToken_ReturnsSingleValueInterpolatedString()
    {
        string source = "first two third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringSegment(source), a)
        );
    }

    [Fact]
    public void Parse_WithFormattedToken_ReturnsInterpolatedString()
    {
        string source = "{two:D}";
        var settings = new StringTokenFormatterSettings {
        };

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringTokenSegment(source, "two", string.Empty, "D"), a)
        );
        Assert.Equal(settings, actual.Settings);
    }

    [Fact]
    public void Parse_WithFormattedAndPaddedToken_ReturnsInterpolatedString()
    {
        string source = "{two,10:D}";
        var settings = new StringTokenFormatterSettings {
        };

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringTokenSegment(source, "two", "10", "D"), a)
        );
        Assert.Equal(settings, actual.Settings);
    }

    [Fact]
    public void Parse_WithEscapedToken_ReturnsInterpolatedString()
    {
        string source = "first ((two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringSegment("first "), a),
            a => Assert.Equal(new InterpolatedStringSegment("("), a),
            a => Assert.Equal(new InterpolatedStringSegment("two) third"), a)
        );
        Assert.Equal(settings, actual.Settings);
    }
}