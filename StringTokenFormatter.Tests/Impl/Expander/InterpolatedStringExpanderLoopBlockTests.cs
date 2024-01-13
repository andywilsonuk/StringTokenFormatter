namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderLoopBlockTests
{
    private readonly BasicContainer valuesContainer = new();
    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderLoopBlockTests()
    {
        settings = StringTokenFormatterSettings.Default with {
            BlockCommands = new List<IBlockCommand>
            {
                BlockCommandFactory.Loop,
            }
        };
    }

    [Fact]
    public void Expand_LoopDataIterations_StringWithLiteralValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void Expand_LoopTokenValueIterations_StringWithLiteralValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", "Iterations", string.Empty),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("Iterations", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void Expand_LoopDataIterations_StringWithTokenValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringTokenSegment("{tok}", "tok", string.Empty, string.Empty),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("tok", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("222", actual);
    }

    [Fact]
    public void Expand_NestedLoop_StringWithLiteralValue3TimesTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("a"),
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "2"),
            new InterpolatedStringLiteralSegment("-"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
            new InterpolatedStringLiteralSegment("b"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("a--ba--ba--b", actual);
    }

    [Fact]
    public void Expand_LoopMissingLoopEndCommand_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("lit"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_LoopMissingLoopCommand_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_LoopWithInvalidType_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "a"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void Expand_LoopSingleIteration_StringWithLiteralValueOnce()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "1"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("lit", actual);
    }

    [Fact]
    public void Expand_LoopZeroIterations_Throw()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", string.Empty, "0"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }
}