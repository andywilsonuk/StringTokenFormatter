namespace StringTokenFormatter.Tests;

public class InterpolatedStringTests
{
    [Fact]
    public void Tokens_ParsedTokens_ReturnsTokenHashset()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("a", string.Empty, string.Empty)
            .Token("A", string.Empty, string.Empty)
            .Token("b", string.Empty, string.Empty)
            .Literal("c")
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);

        HashSet<string> actual = interpolatedString.Tokens();

        HashSet<string> expected = ["a", "b"];
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_CommandSegment_HashsetIncludeCommandToken()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "a", string.Empty)
            .Token("b", string.Empty, string.Empty)
            .Command("ifend", "a", string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);

        HashSet<string> actual = interpolatedString.Tokens();

        HashSet<string> expected = ["a", "b"];
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_CommandSegmentWithEndToken_HashsetIncludeCommandToken()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("if", "a", string.Empty)
            .Token("b", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);

        HashSet<string> actual = interpolatedString.Tokens();

        HashSet<string> expected = ["a", "b"];
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Tokens_ParsedTokensCaseSensitive_ReturnsTokenHashset()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("a", string.Empty, string.Empty)
            .Token("A", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            NameComparer = StringComparer.Ordinal,
        };
        InterpolatedString interpolatedString = new(segments, settings);

        HashSet<string> actual = interpolatedString.Tokens();

        HashSet<string> expected = ["a", "A"];
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToRawString_SegmentConcatination_ReturnsSingleCombinedString()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("a")
            .Token("b", string.Empty, string.Empty)
            .Pseudo("c", string.Empty, string.Empty)
            .Command("d", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, StringTokenFormatterSettings.Default);

        string actual = interpolatedString.ToRawString();

        Assert.Equal("a{b}{::c}{:d}", actual);
    }
}