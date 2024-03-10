namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderTests
{
    private readonly BasicContainer valuesContainer = new();

    [Fact]
    public void WithoutTokens_ReturnsOriginalString()
    {
        var segments = new SegmentBuilder()
            .Literal("text only")
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("text only", actual);
    }

    [Fact]
    public void WithToken_ReturnsExpandedString()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Token("two", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Theory]
    [InlineData("two", "10", "D4", "      0002")]
    [InlineData("two", "", "D4", "0002")]
    [InlineData("two", "10", "", "         2")]
    public void FormattingAndAlignmentToken_ReturnsExpandedString(string token, string alignment, string format, string expected)
    {
        var segments = new SegmentBuilder()
            .Token(token, alignment, format)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UnresolvedTokenBehaviorThrow_ThrowsException()
    {
        var settings = new StringTokenFormatterSettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Throw,
        };
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Token("three", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        Assert.Throws<UnresolvedTokenException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void UnresolvedTokenBehaviorLeaveUnresolved_OutputsRaw()
    {
        var settings = new StringTokenFormatterSettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Token("three", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one {three}", actual);
    }

    [Theory]
    [InlineData("en-GB", "-16325.62")]
    [InlineData("de-DE", "-16325,62")]
    public void FormatProvider_ReturnsExpandedStringUsingProviderSettings(string cultureName, string expected)
    {
        var settings = new StringTokenFormatterSettings
        {
            FormatProvider = CultureInfo.CreateSpecificCulture(cultureName),
        };
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", -16325.62m);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("two", "10", "#.##", "      2,12")]
    [InlineData("two", "", "#.##", "2,12")]
    [InlineData("two", "10", "", "      2,12")]
    public void FormatProviderFormattingAndAlignmentToken_ReturnsExpandedString(string token, string alignment, string format, string expected)
    {
        var settings = new StringTokenFormatterSettings
        {
            FormatProvider = CultureInfo.CreateSpecificCulture("de-DE"),
        };
        var segments = new SegmentBuilder()

            .Token(token, alignment, format)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2.12m);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void BlankValue_ReturnsEmptyString(string? tokenValue)
    {
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", tokenValue);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Empty(actual);
    }

    [Fact]
    public void ValueConverters_ConvertsContainerValueToBinary()
    {
        TokenValueConverter base2Converter = (v, _n) => v is int base10 ? TryGetResult.Success(Convert.ToString(base10, 2)) : default;
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var settings = StringTokenFormatterSettings.Default with
        {
            ValueConverters = new TokenValueConverter[] { base2Converter },
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

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

        var settings = new StringTokenFormatterSettings
        {
            ValueConverters = StringTokenFormatterSettings.Default.ValueConverters.Concat(new TokenValueConverter[] { converter }).ToList().AsReadOnly()
        };
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void UnmatchedValueConverters_Throws()
    {
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var settings = StringTokenFormatterSettings.Default with
        {
            ValueConverters = new[] { TokenValueConverterFactory.NullConverter() },
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", "2");

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ObjectTypeTokenValueWithoutValueConverter_Throws()
    {
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", new { });

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void TokenFuncGivenTokenName_ConvertsContainerValue()
    {
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        Func<string, object?> func = token => token == "two" ? 2 : null;
        valuesContainer.Add("two", func);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void FormatError_Throws()
    {
        var settings = new StringTokenFormatterSettings
        {
            InvalidFormatBehavior = InvalidFormatBehavior.Throw,
        };
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, "Z")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void FormatErrorLeaveUnformatted_StringWithTokenValue()
    {
        var settings = new StringTokenFormatterSettings
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveUnformatted,
        };
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, "Z")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void FormatErrorLeaveToken_StringWithTokenValue()
    {
        var settings = new StringTokenFormatterSettings
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveToken,
        };
        var segments = new SegmentBuilder()
            .Token("two", string.Empty, "Z")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(segments[0].Raw, actual);
    }

    [Fact]
    public void UnresolvedPseudoBehaviorLeaveUnresolved_OutputsRaw()
    {
        var settings = new StringTokenFormatterSettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var segments = new SegmentBuilder()
            .Pseudo("unknown", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(segments[0].Raw, actual);
    }
}