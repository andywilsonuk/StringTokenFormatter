namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderFormatterTests
{
    private readonly BasicContainer valuesContainer = new();

    public InterpolatedStringExpanderFormatterTests()
    {
        valuesContainer.Add("one", 1);
        valuesContainer.Add("two", 2);
    }

    [Fact]
    public void Expand_WithTypeOnlyFormatter_ReturnsXTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{one}", "one", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTypeOnly((decimal value, string formatString) => new string(formatString[0], (int)value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", (decimal)2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("1xx", actual);
    }

    [Fact]
    public void Expand_WithTokenNameFormatter_ReturnsXTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{one}", "one", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTokenName("two", (int value, string formatString) => new string(formatString[0], value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("1xx", actual);
    }

    [Fact]
    public void Expand_WithFormatStringFormatter_ReturnsXTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForFormatString("x", (int value, string formatString) => new string(formatString[0], value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void Expand_WithtokenNameAndFormatStringFormatter_ReturnsXTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTokenNameAndFormatString("two", "x", (int value, string formatString) => new string(formatString[0], value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void Expand_OrderOfPrecedence_ReturnsABCD()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{tok0}", "tok0", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{tok1}", "tok1", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{tok0}", "tok0", string.Empty, "x"),
            new InterpolatedStringTokenSegment("{tok2}", "tok2", string.Empty, "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTypeOnly((int value, string formatString) => "a"),
                FormatterDefinition.ForTokenName("tok1", (int value, string formatString) => "b"),
                FormatterDefinition.ForFormatString("x", (int value, string formatString) => "c"),
                FormatterDefinition.ForTokenNameAndFormatString("tok2", "x", (int value, string formatString) => "d"),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("tok0", 0);
        valuesContainer.Add("tok1", 1);
        valuesContainer.Add("tok2", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("abcd", actual);
    }

    [Fact]
    public void Expand_DifferingTypes_ReturnsXTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTypeOnly((int value, string formatString) => "wrong"),
                FormatterDefinition.ForTypeOnly((decimal value, string formatString) => new string(formatString[0], (int)value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", (decimal)2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void Expand_WithAlignment_ReturnsPaddedXTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", "5", "x"),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTokenName("two", (int value, string formatString) => new string(formatString[0], value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("   xx", actual);
    }

    [Fact]
    public void Expand_WithInvalidAlignment_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", "bad", string.Empty),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTypeOnly((int value, string formatString) => "a"),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_WithThrowingFormatter_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", "bad", string.Empty),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTypeOnly((int value, string formatString) => throw new Exception()),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }
    [Fact]
    public void Expand_DuplicateDefinition_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var settings = StringTokenFormatterSettings.Default with {
            FormatterDefinitions = new List<FormatterDefinition>
            {
                FormatterDefinition.ForTypeOnly((int value, string formatString) => new string(formatString[0], value)),
                FormatterDefinition.ForTypeOnly((int value, string formatString) => new string(formatString[0], value)),
            }
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }
}