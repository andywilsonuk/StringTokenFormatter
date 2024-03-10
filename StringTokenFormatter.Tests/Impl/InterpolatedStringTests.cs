namespace StringTokenFormatter.Tests;

public class InterpolatedStringTests
{
    [Fact]
    public void Tokens_ParsedTokens_ReturnsTokenHashset()
    {
        var segments = new SegmentBuilder()
            .Token("a", string.Empty, string.Empty)
            .Token("A", string.Empty, string.Empty)
            .Token("b", string.Empty, string.Empty)
            .Literal("c")
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);

        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_CommandSegment_HashsetIncludeCommandToken()
    {
        var segments = new SegmentBuilder()
            .Command("if", "a", string.Empty)
            .Token("b", string.Empty, string.Empty)
            .Command("ifend", "a", string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);

        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_CommandSegmentWithEndToken_HashsetIncludeCommandToken()
    {
        var segments = new SegmentBuilder()
            .Command("if", "a", string.Empty)
            .Token("b", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);

        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "b" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_ParsedTokensCaseSensitive_ReturnsTokenHashset()
    {
        var segments = new SegmentBuilder()
            .Token("a", string.Empty, string.Empty)
            .Token("A", string.Empty, string.Empty)
            .Build();
        var settings = StringTokenFormatterSettings.Default with
        {
            NameComparer = StringComparer.Ordinal,
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = interpolatedString.Tokens();

        var expected = new HashSet<string> { "a", "A" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToRawString_SegmentConcatination_ReturnsSingleCombinedString()
    {
        var segments = new SegmentBuilder()
            .Literal("a")
            .Token("b", string.Empty, string.Empty)
            .Pseudo("c", string.Empty, string.Empty)
            .Command("d", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, StringTokenFormatterSettings.Default);

        var actual = interpolatedString.ToRawString();

        Assert.Equal("a{b}{::c}{:d}", actual);
    }
}