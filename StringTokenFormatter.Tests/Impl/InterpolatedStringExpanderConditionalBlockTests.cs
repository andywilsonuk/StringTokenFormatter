namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderConditionalBlockTests
{
    private readonly BasicContainer valuesContainer = new();

    
    [Fact]
    public void Expand_ConditionLiteralValue_StringWithLiteralValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringBlockSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two", actual);
    }

    [Fact]
    public void Expand_ConditionTokenValue_StringWithTokenValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringBlockSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringTokenSegment("{two}", "two", string.Empty, string.Empty),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("two", 2);
        valuesContainer.Add("IsValid", true);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one 2", actual);
    }

    [Fact]
    public void Expand_FalseCondition_StringWithoutValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringBlockSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("IsValid", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one ", actual);
    }

    [Fact]
    public void Expand_NestedConditions_StringWithNestedValue()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringBlockSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringBlockSegment("{:if,IsNotValid}", "if", "IsNotValid", string.Empty),
            new InterpolatedStringLiteralSegment("suppressed"),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringBlockSegment("{:if,IsAlsoValid}", "if", "IsAlsoValid", string.Empty),
            new InterpolatedStringLiteralSegment(" three"),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("IsValid", true);
        valuesContainer.Add("IsAlsoValid", true);
        valuesContainer.Add("IsNotValid", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("one two three", actual);
    }

    [Fact]
    public void Expand_ConditionMissingEndIf_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringBlockSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("IsValid", true);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_ConditionInvalidTokenValue_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("one "),
            new InterpolatedStringBlockSegment("{:if,IsValid}", "if", "IsValid", string.Empty),
            new InterpolatedStringBlockSegment("{:ifend}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        valuesContainer.Add("IsValid", 1);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }
}