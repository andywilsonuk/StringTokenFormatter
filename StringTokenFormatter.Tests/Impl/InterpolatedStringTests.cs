namespace StringTokenFormatter.Tests;

public class InterpolatedStringTests
{
    [Fact]
    public void Tokens_ParsedTokens_ReturnsTokenHashset()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{a}", "a", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{A}", "A", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{b}", "b", string.Empty, string.Empty),
            new InterpolatedStringSegment("c"),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        
        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Tokens_Conditional_HashsetIncludeConditionalToken()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{if>a}", "if>a", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{b}", "b", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{endif>a}", "endif>a", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        
        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equivalent(expected, actual);
    }
}