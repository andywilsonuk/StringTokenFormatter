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
                ExpanderCommandFactory.StandardToken,
                ExpanderCommandFactory.StandardLiteral,
            }
        };
    }

    [Fact]
    public void ConditionLiteralValue_StringWithLiteralValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two", actual);
    }

    [Fact]
    public void ConditionTokenValue_StringWithTokenValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("two", 2);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Fact]
    public void FalseCondition_StringWithoutValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one ", actual);
    }

    [Fact]
    public void NestedConditions_StringWithNestedValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringCommandSegment("{:if,IsNotValid}", "if", "IsNotValid", string.Empty),
            new InterpolatedStringLiteralSegment("suppressed"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:if,IsAlsoValid}", "if", "IsAlsoValid", string.Empty),
            new InterpolatedStringLiteralSegment(" three"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
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
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionMissingIfCommand_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionInvalidTokenValue_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", 1);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void ConditionWithEmptyInner_EmptyString()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void NegatedCondition_StringWithoutLiteralValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one"),
            new InterpolatedStringCommandSegment("{:if,!IsValid}", "if", "!IsValid", string.Empty),
            new InterpolatedStringLiteralSegment(" two"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one", actual);
    }


    [Fact]
    public void ConditionalPreventsInnerTokenMatching_StringWithoutSuppressedValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:if,!IsValid}", "if", "!IsValid", string.Empty),
            new InterpolatedStringTokenSegment("{Suppressed}", "Suppressed", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        valuesContainer.Add("IsValid", true);
        valuesContainer.Add("Suppressed", () => Assert.Fail("This value should not be called"));

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void ConditionalFromFunc_StringWithoutSuppressedValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("Included"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        valuesContainer.Add("IsValid", () => true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("Included", actual);
    }

    [Fact]
    public void MultipleConditionsIncludingNegation_LiteralsOneAndFourOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("one"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:if,!IsValid}", "if", "!IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:if,!IsValid}", "if", "!IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("three"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("four"),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("onefour", actual);
    }
}