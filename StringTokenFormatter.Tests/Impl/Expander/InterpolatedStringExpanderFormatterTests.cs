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
    public void WithTypeOnlyFormatter_OutputXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("one", string.Empty, string.Empty)
            .Token("two", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((decimal value, string formatString) => new string(formatString[0], (int)value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", (decimal)2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("1xx", actual);
    }

    [Fact]
    public void WithTokenNameFormatter_OutputXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("one", string.Empty, string.Empty)
            .Token("two", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForTokenName("two", (int value, string formatString) => new string(formatString[0], value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("1xx", actual);
    }

    [Fact]
    public void WithFormatStringFormatter_OutputXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForFormatString("x", (int value, string formatString) => new string(formatString[0], value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void WithTokenNameAndFormatStringFormatter_OutputXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForTokenNameAndFormatString("two", "x", (int value, string formatString) => new string(formatString[0], value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void OrderOfPrecedence_OutputABCD()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("tok0", string.Empty, string.Empty)
            .Token("tok1", string.Empty, string.Empty)
            .Token("tok0", string.Empty, "x")
            .Token("tok2", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((int value, string formatString) => "a"),
                FormatterDefinition.ForTokenName("tok1", (int value, string formatString) => "b"),
                FormatterDefinition.ForFormatString("x", (int value, string formatString) => "c"),
                FormatterDefinition.ForTokenNameAndFormatString("tok2", "x", (int value, string formatString) => "d"),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("tok0", 0);
        valuesContainer.Add("tok1", 1);
        valuesContainer.Add("tok2", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("abcd", actual);
    }

    [Fact]
    public void DifferingTypes_OutputXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((int value, string formatString) => "wrong"),
                FormatterDefinition.ForType((decimal value, string formatString) => new string(formatString[0], (int)value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", (decimal)2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void WithAlignment_OutputPaddedXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", "5", "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForTokenName("two", (int value, string formatString) => new string(formatString[0], value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("   xx", actual);
    }

    [Fact]
    public void WithInvalidAlignment_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", "bad", string.Empty)
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((int value, string formatString) => "a"),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void WithThrowingFormatter_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", "bad", string.Empty)
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((int value, string formatString) => throw new Exception()),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void SpecificTypeNotObject_OutputXTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "x")
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((object value, string formatString) => "wrong"),
                FormatterDefinition.ForType((int value, string formatString) => new string(formatString[0], value)),
            ]
        };
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("xx", actual);
    }

    [Fact]
    public void NoMatchingDefinition_FormatsWithProvider()
    {
        StringTokenFormatterSettings settings = new()
        {
            FormatProvider = CultureInfo.CreateSpecificCulture("en-GB"),
            FormatterDefinitions =
            [
                FormatterDefinition.ForType((int value, string formatString) => "wrong"),
            ]
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", -16325.62m);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("-16325.62", actual);
    }
}
