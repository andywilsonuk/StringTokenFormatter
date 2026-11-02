namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderLoopBlockTests
{
    private readonly BasicContainer valuesContainer = new();
    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderLoopBlockTests()
    {
        settings = StringTokenFormatterSettings.Default with
        {
            Commands =
            [
                ExpanderCommandFactory.Loop,
                ExpanderCommandFactory.Standard,
            ]
        };
    }

    [Fact]
    public void LoopDataIterations_StringWithLiteralValue3Times()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void LoopTokenValueIterations_StringWithLiteralValue3Times()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterations", string.Empty)
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("Iterations", 3);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void LoopDataIterationsUsedOverTokenValue_StringWithLiteralValue3Times()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterations", "2")
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("Iterations", 3);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlit", actual);
    }

    [Fact]
    public void LoopDataIterations_StringWithTokenValue3Times()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Token("tok", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("tok", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("222", actual);
    }

    [Fact]
    public void NestedLoop_StringWithLiteralValue3TimesTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Literal("a")
            .Command("loop", string.Empty, "2")
            .Literal("-")
            .Command("loopend", string.Empty, string.Empty)
            .Literal("b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("a--ba--ba--b", actual);
    }

    [Fact]
    public void LoopMissingLoopEndCommand_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Literal("lit")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void LoopMissingLoopCommand_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void LoopWithInvalidType_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "a")
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void LoopSingleIteration_StringWithLiteralValueOnce()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "1")
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("lit", actual);
    }

    [Fact]
    public void LoopZeroIterations_Throw()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "0")
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void LoopUsingTokenFunc_EachValueIsOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "2")
            .Token("Val", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();

        int callCount = 0;
        Func<string> calls = () =>
        {
            callCount++;
            return callCount switch
            {
                1 => "a",
                2 => "b",
                _ => throw new IndexOutOfRangeException("Too many calls"),
            };
        };

        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("Val", calls);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("ab", actual);
    }

    [Fact]
    public void LoopUsingSequenceOfPrimatives_EachValueIsOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator", "4", string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", ["a", "b"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("   a   b", actual);
    }

    [Fact]
    public void LoopUsingSequenceOfObjects_EachValueIsOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator.Name", "4", string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { Name = "a" });
        var o2 = TokenValueContainerFactory.FromObject(settings, new { Name = "b" });
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [o1, o2]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("   a   b", actual);
    }

    [Fact]
    public void LoopUsingEmptySequence_OutputsEmpty()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator", "4", string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", Array.Empty<string>());
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void LoopUsingRestrictedIterations_OnlyFirstItemOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", "1")
            .Token("Iterator", "4", string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", ["a", "b"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("   a", actual);
    }

    [Fact]
    public void LoopPseudoLoopIterationWithFormat_FormattedLoopIterationOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Token("::loopiteration", "4", "D2")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("  01  02  03", actual);
    }

    [Fact]
    public void EmptyLoopInner_EmptyString()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void LoopTokenValueIterationsOutputIterationsToken_OutputIterationsValueOnEachIteration()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterations", string.Empty)
            .Token("Iterations", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("Iterations", 3);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("333", actual);
    }

    [Fact]
    public void LoopWhenNestedTokenNotFound_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator.Name", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { NotName = "a" });
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [o1]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<UnresolvedTokenException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopUsingBadValueConversion_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [new object()]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopUsingNestedBadValueConversion_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator.Name", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { Name = new object() });
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [o1]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopWhenNestedTokenNotFoundWithLeaveUnresolved_OutputsRaw()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Token("Iterator.Name", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        InterpolatedString interpolatedString = new(segments, customSettings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { NotName = "a" });
        var o2 = TokenValueContainerFactory.FromObject(settings, new { NotName = "b" });
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [o1, o2]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("{Iterator.Name}{Iterator.Name}", actual);
    }

    [Fact]
    public void LoopValueWhenNotInLoop_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Token("Iterator", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [new object()]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopIterationWhenNotInLoop_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Pseudo("loopiteration", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [new object()]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void NestedLoopWithLoopIterationPseudo_OutputIterationOfInnerLoop()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Literal("a")
            .Command("loop", string.Empty, "2")
            .Pseudo("loopiteration", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Literal("b")
            .Pseudo("loopiteration", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("a12b1a12b2a12b3", actual);
    }

    [Fact]
    public void NestedLoopWithTwoSequences_OutputValuesFromEitherSequence()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "S1", string.Empty)
            .Command("loop", "S2", string.Empty)
            .Token("S1", string.Empty, string.Empty)
            .Token("S2", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence1 = TokenValueContainerFactory.FromSequence(settings, "S1", ["a", "b"]);
        SequenceTokenValueContainer sequence2 = TokenValueContainerFactory.FromSequence(settings, "S2", ["c"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence1, sequence2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("acbc", actual);
    }

    [Fact]
    public void IfConditionUsingSequenceValue_SecondNameOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("if", "Iterator.IsValue", string.Empty)
            .Token("Iterator.Name", string.Empty, string.Empty)
            .Command("ifend", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings customSettings = StringTokenFormatterSettings.Default with
        {
            Commands = new[] { ExpanderCommandFactory.Conditional }.Concat(settings.Commands).ToList(),
        };
        InterpolatedString interpolatedString = new(segments, customSettings);
        var o1 = TokenValueContainerFactory.FromObject(customSettings, new { IsValue = false, Name = "a" });
        var o2 = TokenValueContainerFactory.FromObject(customSettings, new { IsValue = true, Name = "b" });
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(customSettings, "Iterator", [o1, o2]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(customSettings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void LoopCount_CountOf2OutputTwice()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Pseudo("loopcount", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", ["a", "b"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("22", actual);
    }

    [Fact]
    public void LoopCountRespectsIterationsData_CountOf2OutputOnce()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", "1")
            .Pseudo("loopcount", string.Empty, string.Empty)
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", ["a", "b"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("1", actual);
    }


    [Fact]
    public void LoopTokenNotFoundWithLeaveUnresolved_OutputsRaw()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterations", string.Empty)
            .Literal("lit")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        StringTokenFormatterSettings customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        InterpolatedString interpolatedString = new(segments, customSettings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(segments[0].Raw, actual);
    }
}