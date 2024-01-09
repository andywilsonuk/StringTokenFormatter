namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderTests
{
    private readonly BasicContainer valuesContainer = new();

    [Fact]
    public void Expand_WithoutTokens_ReturnsOriginalString()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("text only"),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("text only", actual);
    }

    [Fact]
    public void Expand_WithToken_ReturnsExpandedString()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Theory]
    [InlineData("{two,10:D4}", "two", "10", "D4", "      0002")]
    [InlineData("{two,D4}", "two", "", "D4", "0002")]
    [InlineData("{two,10}", "two", "10", "", "         2")]
    public void Expand_FormattingAndAlignmentToken_ReturnsExpandedString(string raw, string token, string alignment, string format, string expected)
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment(raw, token, alignment, format),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Expand_UnresolvedTokenBehaviorThrow_ThrowsException()
    {
        var settings = new StringTokenFormatterSettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Throw,
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringTokenSegment("{three}", "three", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        Assert.Throws<UnresolvedTokenException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_UnresolvedTokenBehaviorLeaveUnresolved_ThrowsException()
    {
        var settings = new StringTokenFormatterSettings
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringTokenSegment("{three}", "three", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one {three}", actual);
    }

    [Theory]
    [InlineData("en-GB", "-16325.62")]
    [InlineData("de-DE", "-16325,62")]
    public void Expand_FormatProvider_ReturnsExpandedStringUsingProviderSettings(string cultureName, string expected)
    {
        var settings = new StringTokenFormatterSettings
        {
            FormatProvider = CultureInfo.CreateSpecificCulture(cultureName),
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", -16325.62m);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("{two,10:#.##}", "two", "10", "#.##", "      2,12")]
    [InlineData("{two,#.##}", "two", "", "#.##", "2,12")]
    [InlineData("{two,10}", "two", "10", "", "      2,12")]
    public void Expand_FormatProviderFormattingAndAlignmentToken_ReturnsExpandedString(string raw, string token, string alignment, string format, string expected)
    {
        var settings = new StringTokenFormatterSettings
        {
            FormatProvider = CultureInfo.CreateSpecificCulture("de-DE"),
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment(raw, token, alignment, format),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2.12m);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Expand_BlankValue_ReturnsEmptyString(string? tokenValue)
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", tokenValue);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Empty(actual);
    }

    [Fact]
    public void Expand_ValueConverters_ConvertsContainerValueToBinary()
    {
        TokenValueConverter base2Converter = (v, _n) => v is int base10 ? TryGetResult.Success(Convert.ToString(base10, 2)) : default;
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var settings = StringTokenFormatterSettings.Default with {
            ValueConverters = new TokenValueConverter[] { base2Converter },
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("10", actual);
    }

    [Fact]
    public void Expand_PrimativeTypeMatchesBeforeFunc_ValueConvertersStopOnceMatched()
    {
        static TryGetResult converter(object? _v, string _n)
        {
            Assert.Fail("This converter should not be reached");
            return TryGetResult.Success(2) ;
        }

        var settings = new StringTokenFormatterSettings
        {
            ValueConverters = StringTokenFormatterSettings.Default.ValueConverters.Concat(new TokenValueConverter[] { converter }).ToList().AsReadOnly()
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void Expand_UnmatchedValueConverters_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var settings = StringTokenFormatterSettings.Default with {
            ValueConverters = new[] { TokenValueConverters.NullConverter() },
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", "2");

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_ObjectTypeTokenValueWithoutValueConverter_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", new {});

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_TokenFuncGivenTokenName_ConvertsContainerValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        Func<string, object?> func = token => token == "two" ? 2 : null;
        valuesContainer.Add("two", func);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void Expand_FormatError_Throws()
    {
        var settings = new StringTokenFormatterSettings
        {
            InvalidFormatBehavior = InvalidFormatBehavior.Throw,
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two,Z}", "two", string.Empty, "Z"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        Assert.Throws<TokenValueFormatException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_FormatErrorLeaveUnformatted_StringWithTokenValue()
    {
        var settings = new StringTokenFormatterSettings
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveUnformatted,
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two,Z}", "two", string.Empty, "Z"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void Expand_FormatErrorLeaveToken_StringWithTokenValue()
    {
         var settings = new StringTokenFormatterSettings
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveToken,
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two,Z}", "two", string.Empty, "Z"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{two,Z}", actual);
    }
}