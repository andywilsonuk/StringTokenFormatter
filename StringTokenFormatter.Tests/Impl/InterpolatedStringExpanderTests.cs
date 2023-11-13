namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderTests
{
    [Fact]
    public void Expand_WithoutTokens_ReturnsOriginalString()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringSegment("text only"),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("text only", actual);
        valuesStub.VerifyNoOtherCalls();
    }

    [Fact]
    public void Expand_WithToken_ReturnsExpandedString()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

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
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

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
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{three}", "three", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        Assert.Throws<UnresolvedTokenException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object));
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
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{three}", "three", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

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
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(-16325.62m));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

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
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2.12m));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Expand_BlankValue_ReturnsEmptyString(string tokenValue)
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(tokenValue));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Empty(actual);
    }

    [Fact]
    public void Expand_ValueConverters_ConvertsContainerValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        Func<object> func = () => 2;
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(func));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("2", actual);
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
            ValueConverters = StringTokenFormatterSettings.Global.ValueConverters.Concat(new TokenValueConverter[] { converter }).ToList().AsReadOnly()
        };
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void Expand_TokenFuncGivenTokenName_ConvertsContainerValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        Func<string, object?> func = token => token == "two" ? 2 : null;
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(func));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void Expand_ConditionLiteralValue_StringWithLiteralValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{if:IsValid}", "if:IsValid", string.Empty, string.Empty),
            new InterpolatedStringSegment("two"),
            new InterpolatedStringTokenSegment("{endif}", "endif", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("IsValid")).Returns(TryGetResult.Success(true));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("one two", actual);
    }

    [Fact]
    public void Expand_ConditionTokenValue_StringWithTokenValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{if:IsValid}", "if:IsValid", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{endif}", "endif", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));
        valuesStub.Setup(x => x.TryMap("IsValid")).Returns(TryGetResult.Success(true));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("one 2", actual);
    }

    [Fact]
    public void Expand_FalseCondition_StringWithoutValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{if:IsValid}", "if:IsValid", string.Empty, string.Empty),
            new InterpolatedStringSegment("two"),
            new InterpolatedStringTokenSegment("{endif}", "endif", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("IsValid")).Returns(TryGetResult.Success(false));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("one ", actual);
    }

    [Fact]
    public void Expand_NestedConditions_StringWithNestedValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringSegment("one "),
            new InterpolatedStringTokenSegment("{if:IsValid}", "if:IsValid", string.Empty, string.Empty),
            new InterpolatedStringSegment("two"),
            new InterpolatedStringTokenSegment("{if:IsNotValid}", "if:IsNotValid", string.Empty, string.Empty),
            new InterpolatedStringSegment("suppressed"),
            new InterpolatedStringTokenSegment("{endif}", "endif", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{if:IsAlsoValid}", "if:IsAlsoValid", string.Empty, string.Empty),
            new InterpolatedStringSegment(" three"),
            new InterpolatedStringTokenSegment("{endif}", "endif", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{endif}", "endif", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Global);
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("IsValid")).Returns(TryGetResult.Success(true));
        valuesStub.Setup(x => x.TryMap("IsAlsoValid")).Returns(TryGetResult.Success(true));
        valuesStub.Setup(x => x.TryMap("IsNotValid")).Returns(TryGetResult.Success(false));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesStub.Object);

        Assert.Equal("one two three", actual);
    }
}