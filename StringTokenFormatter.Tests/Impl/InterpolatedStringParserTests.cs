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
            a => Assert.Equal(new InterpolatedStringLiteralSegment("first "), a),
            a => Assert.Equal(new InterpolatedStringTokenSegment("(two)", "two", string.Empty, string.Empty), a),
            a => Assert.Equal(new InterpolatedStringLiteralSegment(" third"), a)
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
            a => Assert.Equal(new InterpolatedStringLiteralSegment(source), a)
        );
    }

    [Fact]
    public void Parse_WithFormattedToken_ReturnsInterpolatedString()
    {
        string source = "{two:D}";
        var settings = new StringTokenFormatterSettings();

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringTokenSegment(source, "two", string.Empty, "D"), a)
        );
    }

    [Fact]
    public void Parse_WithFormattedAndPaddedToken_ReturnsInterpolatedString()
    {
        string source = "{two,10:D}";
        var settings = new StringTokenFormatterSettings();

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringTokenSegment(source, "two", "10", "D"), a)
        );
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
            a => Assert.Equal(new InterpolatedStringLiteralSegment("first "), a),
            a => Assert.Equal(new InterpolatedStringLiteralSegment("("), a),
            a => Assert.Equal(new InterpolatedStringLiteralSegment("two) third"), a)
        );
    }

    [Fact]
    public void Parse_WithBlankToken_Throws()
    {
        string source = "first {} third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Curly,
        };

        Assert.Throws<ParserException>(() => InterpolatedStringParser.Parse(source, settings));
    }

    [Fact]
    public void Parse_WithInvalidSyntax_Throws()
    {
        string source = "first {a} third";
        var settings = new StringTokenFormatterSettings {
            Syntax = new TokenSyntax(string.Empty, "}", "{{"),
        };

        Assert.Throws<ArgumentException>(() => InterpolatedStringParser.Parse(source, settings));
    }

    [Fact]
    public void Parse_WithBlockCommand_ReturnsBlockCommandSegment()
    {
        string source = "{:cmd}";
        var settings = new StringTokenFormatterSettings();

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringBlockSegment(source, "cmd", string.Empty, string.Empty), a)
        );
    }

    [Fact]
    public void Parse_WithBlockCommandToken_ReturnsBlockCommandSegmentWithToken()
    {
        string source = "{:cmd,token}";
        var settings = new StringTokenFormatterSettings();

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringBlockSegment(source, "cmd", "token", string.Empty), a)
        );
    }

    [Fact]
    public void Parse_WithBlockCommandData_ReturnsBlockCommandSegmentWithData()
    {
        string source = "{:cmd:4}";
        var settings = new StringTokenFormatterSettings();

        var actual = InterpolatedStringParser.Parse(source, settings);

        Assert.Collection(actual.Segments,
            a => Assert.Equal(new InterpolatedStringBlockSegment(source, "cmd", string.Empty, "4"), a)
        );
    }
}