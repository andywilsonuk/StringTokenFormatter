namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderConditionalBlockTests
{
    private readonly BasicContainer valuesContainer = new();
    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderConditionalBlockTests()
    {
        settings = StringTokenFormatterSettings.Default with
        {
            Commands =
            [
                ExpanderCommandFactory.Conditional,
                ExpanderCommandFactory.Standard,
            ]
        };
    }

    [Fact]
    public void ConditionLiteralValue_StringWithLiteralValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", true);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two", actual);
    }

    [Fact]
    public void ConditionTokenValue_StringWithTokenValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Token("two", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("two", 2);
        valuesContainer.Add("IsValid", true);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Fact]
    public void FalseCondition_StringWithoutValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", false);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one ", actual);
    }

    [Fact]
    public void NestedConditions_StringWithNestedValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("if", "IsNotValid", string.Empty)
            .Literal("suppressed")
            .Command("ifend", string.Empty, string.Empty)
            .Command("if", "IsAlsoValid", string.Empty)
            .Literal(" three")
            .Command("ifend", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", true);
        valuesContainer.Add("IsAlsoValid", true);
        valuesContainer.Add("IsNotValid", false);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two three", actual);
    }

    [Fact]
    public void ConditionMissingEndIfCommand_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", true);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionMissingIfCommand_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionInvalidTokenValue_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", 1);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionWithEmptyInner_EmptyString()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", true);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void NegatedCondition_StringWithoutLiteralValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("one")
            .Command("if", "!IsValid", string.Empty)
            .Literal(" two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", true);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one", actual);
    }


    [Fact]
    public void ConditionalPreventsInnerTokenMatching_StringWithoutSuppressedValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "!IsValid", string.Empty)
            .Token("Suppressed", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        valuesContainer.Add("IsValid", true);
        valuesContainer.Add("Suppressed", () => Assert.Fail("This value should not be called"));

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void ConditionalFromFunc_StringWithoutSuppressedValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Literal("Included")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        valuesContainer.Add("IsValid", () => true);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("Included", actual);
    }

    [Fact]
    public void MultipleConditionsIncludingNegation_LiteralsOneAndFourOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Literal("one")
            .Command("ifend", string.Empty, string.Empty)
            .Command("if", "!IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Command("if", "!IsValid", string.Empty)
            .Literal("three")
            .Command("ifend", string.Empty, string.Empty)
            .Command("if", "IsValid", string.Empty)
            .Literal("four")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("IsValid", true);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("onefour", actual);
    }

    [Fact]
    public void UnresolvedConditionalTokenWithLeaveUnresolved_OutputsRawAndInner()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        InterpolatedString interpolatedString = new(segments, customSettings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:if,IsValid}two", actual);
    }
}