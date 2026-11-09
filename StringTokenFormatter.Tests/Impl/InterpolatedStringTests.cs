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
        InterpolatedString interpolatedString = InterpolatedString.Create(segments, StringTokenFormatterSettings.Default);

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
        InterpolatedString interpolatedString = InterpolatedString.Create(segments, StringTokenFormatterSettings.Default);

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
        InterpolatedString interpolatedString = InterpolatedString.Create(segments, StringTokenFormatterSettings.Default);

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
        InterpolatedString interpolatedString = InterpolatedString.Create(segments, settings);

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
        InterpolatedString interpolatedString = InterpolatedString.Create(segments, StringTokenFormatterSettings.Default);

        string actual = interpolatedString.ToRawString();

        Assert.Equal("a{b}{::c}{:d}", actual);
    }

    [Fact]
    public void Create_OwnsReadOnlyIndexedCopyOfSegments()
    {
        List<InterpolatedStringSegment> source =
        [
            new InterpolatedStringLiteralSegment("before"),
        ];

        InterpolatedString actual = InterpolatedString.Create(source, StringTokenFormatterSettings.Default);

        source[0] = new InterpolatedStringLiteralSegment("after");
        source.Add(new InterpolatedStringLiteralSegment(" added"));

        Assert.Single(actual.Segments);
        Assert.Equal("before", actual.Segments[0].Raw);
        Assert.Throws<NotSupportedException>(() =>
            ((IList<InterpolatedStringSegment>)actual.Segments)[0] = new InterpolatedStringLiteralSegment("changed"));
    }

    [Fact]
    public void CreateEmpty_UsesSharedEmptySegments()
    {
        InterpolatedString actual = InterpolatedString.CreateEmpty(StringTokenFormatterSettings.Default);
        InterpolatedString another = InterpolatedString.CreateEmpty(StringTokenFormatterSettings.Default);
        InterpolatedString fromEmptyCollection = InterpolatedString.Create(
            Array.Empty<InterpolatedStringSegment>(),
            StringTokenFormatterSettings.Default);

        Assert.Empty(actual.Segments);
        Assert.Same(actual.Segments, another.Segments);
        Assert.Same(actual.Segments, fromEmptyCollection.Segments);
    }
}