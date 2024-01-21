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
            new InterpolatedStringLiteralSegment("c"),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        
        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_BlockSegment_HashsetIncludeBlockToken()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:if,a}", "if", "a", string.Empty),
            new InterpolatedStringTokenSegment("{b}", "b", string.Empty, string.Empty),
            new InterpolatedStringBlockSegment("{:ifend,a}", "ifend", "a", string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        
        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_BlockSegmentWithEndToken_HashsetIncludeBlockToken()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:if,a}", "if", "a", string.Empty),
            new InterpolatedStringTokenSegment("{b}", "b", string.Empty, string.Empty),
            new InterpolatedStringBlockSegment("{:ifend,a}", "ifend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);
        
        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_ParsedTokensCaseSensitive_ReturnsTokenHashset()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{a}", "a", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{A}", "A", string.Empty, string.Empty),
        };
        var settings = StringTokenFormatterSettings.Default with {
            NameComparer = StringComparer.Ordinal,
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        
        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "A" };
        Assert.Equal(expected, actual);
    }
}