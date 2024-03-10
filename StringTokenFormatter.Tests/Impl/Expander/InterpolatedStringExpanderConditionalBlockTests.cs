namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderConditionalBlockTests
{
    private readonly BasicContainer valuesContainer = new();
    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderConditionalBlockTests()
    {
        settings = StringTokenFormatterSettings.Default with
        {
            Commands = new List<IExpanderCommand>
            {
                ExpanderCommandFactory.Conditional,
                ExpanderCommandFactory.Standard,
            }
        };
    }

    [Fact]
    public void ConditionLiteralValue_StringWithLiteralValue()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two", actual);
    }

    [Fact]
    public void ConditionTokenValue_StringWithTokenValue()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Token("two", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Fact]
    public void FalseCondition_StringWithoutValue()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one ", actual);
    }

    [Fact]
    public void NestedConditions_StringWithNestedValue()
    {
        var segments = new SegmentBuilder()
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
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);
        valuesContainer.Add("IsAlsoValid", true);
        valuesContainer.Add("IsNotValid", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two three", actual);
    }

    [Fact]
    public void ConditionMissingEndIfCommand_Throws()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionMissingIfCommand_Throws()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionInvalidTokenValue_Throws()
    {
        var segments = new SegmentBuilder()
            .Literal("one ")
            .Command("if", "IsValid", string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", 1);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionWithEmptyInner_EmptyString()
    {
        var segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void NegatedCondition_StringWithoutLiteralValue()
    {
        var segments = new SegmentBuilder()
            .Literal("one")
            .Command("if", "!IsValid", string.Empty)
            .Literal(" two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one", actual);
    }


    [Fact]
    public void ConditionalPreventsInnerTokenMatching_StringWithoutSuppressedValue()
    {
        var segments = new SegmentBuilder()
            .Command("if", "!IsValid", string.Empty)
            .Token("Suppressed", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);

        valuesContainer.Add("IsValid", true);
        valuesContainer.Add("Suppressed", () => Assert.Fail("This value should not be called"));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void ConditionalFromFunc_StringWithoutSuppressedValue()
    {
        var segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Literal("Included")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);

        valuesContainer.Add("IsValid", () => true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("Included", actual);
    }

    [Fact]
    public void MultipleConditionsIncludingNegation_LiteralsOneAndFourOutput()
    {
        var segments = new SegmentBuilder()
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
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("onefour", actual);
    }

    [Fact]
    public void UnresolvedConditionalTokenWithLeaveUnresolved_OutputsRawAndInner()
    {
        var segments = new SegmentBuilder()
            .Command("if", "IsValid", string.Empty)
            .Literal("two")
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var interpolatedString = new InterpolatedString(segments, customSettings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:if,IsValid}two", actual);
    }
}