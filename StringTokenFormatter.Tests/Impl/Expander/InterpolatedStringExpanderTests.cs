namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderTests
{
    private readonly BasicContainer valuesContainer = new();

    [Fact]
    public void WithoutTokens_ReturnsOriginalString()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("text only")
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("text only", actual);
    }

    [Fact]
    public void WithToken_ReturnsExpandedString()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Theory]
    [InlineData("two", "10", "D4", "      0002")]
    [InlineData("two", "", "D4", "0002")]
    [InlineData("two", "10", "", "         2")]
    public void FormattingAndAlignmentToken_ReturnsExpandedString(string token, string alignment, string format, string expected)
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token(token, alignment, format)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UnresolvedTokenBehaviorThrow_ThrowsException()
    {
        StringTokenFormatterSettings settings = new()
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Throw,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Token("three", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        Assert.Throws<UnresolvedTokenException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void UnresolvedTokenBehaviorLeaveUnresolved_OutputsRaw()
    {
        StringTokenFormatterSettings settings = new()
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Token("three", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one {three}", actual);
    }

    [Theory]
    [InlineData("en-GB", "-16325.62")]
    [InlineData("de-DE", "-16325,62")]
    public void FormatProvider_ReturnsExpandedStringUsingProviderSettings(string cultureName, string expected)
    {
        StringTokenFormatterSettings settings = new()
        {
            FormatProvider = CultureInfo.CreateSpecificCulture(cultureName),
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", -16325.62m);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("two", "10", "#.##", "      2,12")]
    [InlineData("two", "", "#.##", "2,12")]
    [InlineData("two", "10", "", "      2,12")]
    public void FormatProviderFormattingAndAlignmentToken_ReturnsExpandedString(string token, string alignment, string format, string expected)
    {
        StringTokenFormatterSettings settings = new()
        {
            FormatProvider = CultureInfo.CreateSpecificCulture("de-DE"),
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()

            .Token(token, alignment, format)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2.12m);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void BlankValue_ReturnsEmptyString(string? tokenValue)
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", tokenValue);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Empty(actual);
    }

    [Fact]
    public void ValueConverters_ConvertsContainerValueToBinary()
    {
        TokenValueConverter base2Converter = (v, _n) => v is int base10 ? TryGetResult.Success(Convert.ToString(base10, 2)) : default;
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            ValueConverters = [base2Converter],
        };
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("10", actual);
    }

    [Fact]
    public void PrimativeTypeMatchesBeforeFunc_ValueConvertersStopOnceMatched()
    {
        static TryGetResult converter(object? _v, string _n)
        {
            Assert.Fail("This converter should not be reached");
            return TryGetResult.Success(2);
        }

        StringTokenFormatterSettings settings = new()
        {
            ValueConverters = StringTokenFormatterSettings.Default.ValueConverters.Concat([converter]).ToList().AsReadOnly()
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void UnmatchedValueConverters_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            ValueConverters = [TokenValueConverterFactory.NullConverter()],
        };
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", "2");

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ObjectTypeTokenValueWithoutValueConverter_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", new { });

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void TokenFuncGivenTokenName_ConvertsContainerValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);
        Func<string, object?> func = token => token == "two" ? 2 : null;
        valuesContainer.Add("two", func);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void FormatError_Throws()
    {
        StringTokenFormatterSettings settings = new()
        {
            InvalidFormatBehavior = InvalidFormatBehavior.Throw,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "Z")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void FormatErrorLeaveUnformatted_StringWithTokenValue()
    {
        StringTokenFormatterSettings settings = new()
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveUnformatted,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "Z")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void FormatErrorLeaveToken_StringWithTokenValue()
    {
        StringTokenFormatterSettings settings = new()
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveToken,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("two", string.Empty, "Z")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(segments[0].Raw, actual);
    }

    [Fact]
    public void UnresolvedPseudoBehaviorLeaveUnresolved_OutputsRaw()
    {
        StringTokenFormatterSettings settings = new()
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Pseudo("unknown", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(segments[0].Raw, actual);
    }
}