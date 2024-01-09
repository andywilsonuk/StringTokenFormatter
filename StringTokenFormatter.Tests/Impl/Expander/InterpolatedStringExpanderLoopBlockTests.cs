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
                BlockCommandFactory.Conditional,
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
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("twotwotwo", actual);
    }

    [Fact]
    public void Expand_LoopTokenValueIterations_StringWithLiteralValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:loop}", "loop", "Iterations", string.Empty),
            new InterpolatedStringLiteralSegment("two"),
            new InterpolatedStringBlockSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("Iterations", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("twotwotwo", actual);
    }

    // nested loops
    // loops and conditional together
}