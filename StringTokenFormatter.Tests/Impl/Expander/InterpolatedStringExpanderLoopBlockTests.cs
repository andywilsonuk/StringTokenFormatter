namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderLoopBlockTests
{
    private readonly BasicContainer valuesContainer = new();
    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderLoopBlockTests()
    {
        settings = StringTokenFormatterSettings.Default with
        {
            Commands = new List<IExpanderCommand>
            {
                ExpanderCommandFactory.Loop,
                ExpanderCommandFactory.Standard,
            }
        };
    }

    [Fact]
    public void LoopDataIterations_StringWithLiteralValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void LoopTokenValueIterations_StringWithLiteralValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterations", string.Empty),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("Iterations", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void LoopDataIterationsUsedOverTokenValue_StringWithLiteralValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterations", "2"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("Iterations", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("litlit", actual);
    }

    [Fact]
    public void LoopDataIterations_StringWithTokenValue3Times()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringTokenSegment("{tok}", "tok", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("tok", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("222", actual);
    }

    [Fact]
    public void NestedLoop_StringWithLiteralValue3TimesTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("a"),
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "2"),
            new InterpolatedStringLiteralSegment("-"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
            new InterpolatedStringLiteralSegment("b"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("a--ba--ba--b", actual);
    }

    [Fact]
    public void LoopMissingLoopEndCommand_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("lit"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void LoopMissingLoopCommand_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void LoopWithInvalidType_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "a"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void LoopSingleIteration_StringWithLiteralValueOnce()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "1"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("lit", actual);
    }

    [Fact]
    public void LoopZeroIterations_Throw()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "0"),
            new InterpolatedStringLiteralSegment("lit"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void LoopUsingTokenFunc_EachValueIsOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "2"),
            new InterpolatedStringTokenSegment("{Val}", "Val", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };

        int callCount = 0;
        var calls = () =>
        {
            callCount++;
            return callCount switch
            {
                1 => "a",
                2 => "b",
                _ => throw new IndexOutOfRangeException("Too many calls"),
            };
        };

        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("Val", calls);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("ab", actual);
    }

    [Fact]
    public void LoopUsingSequenceOfPrimatives_EachValueIsOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator}", "Iterator", "4", string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { "a", "b" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("   a   b", actual);
    }

    [Fact]
    public void LoopUsingSequenceOfObjects_EachValueIsOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator.Name}", "Iterator.Name", "4", string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { Name = "a" });
        var o2 = TokenValueContainerFactory.FromObject(settings, new { Name = "b" });
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { o1, o2 });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("   a   b", actual);
    }

    [Fact]
    public void LoopUsingEmptySequence_OutputsEmpty()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator}", "Iterator", "4", string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", Array.Empty<string>());
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void LoopUsingRestrictedIterations_OnlyFirstItemOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", "1"),
            new InterpolatedStringTokenSegment("{Iterator}", "Iterator", "4", string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { "a", "b" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("   a", actual);
    }

    [Fact]
    public void LoopPseudoLoopIterationWithFormat_FormattedLoopIterationOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringTokenSegment("{::loopiteration}", "::loopiteration", "4", "D2"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("  01  02  03", actual);
    }

    [Fact]
    public void EmptyLoopInner_EmptyString()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void LoopTokenValueIterationsOutputIterationsToken_OutputIterationsValueOnEachIteration()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterations", string.Empty),
            new InterpolatedStringTokenSegment("{Iterations}", "Iterations", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("Iterations", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("333", actual);
    }

    [Fact]
    public void LoopWhenNestedTokenNotFound_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator.Name}", "Iterator.Name", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { NotName = "a" });
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { o1 });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<UnresolvedTokenException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopUsingBadValueConversion_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator}", "Iterator", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { new object() });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopUsingNestedBadValueConversion_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator.Name}", "Iterator.Name", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { Name = new object() });
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { o1 });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<MissingValueConverterException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopWhenNestedTokenNotFoundWithLeaveUnresolved_OutputsRaw()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator.Name}", "Iterator.Name", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var interpolatedString = new InterpolatedString(segments, customSettings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { NotName = "a" });
        var o2 = TokenValueContainerFactory.FromObject(settings, new { NotName = "b" });
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { o1, o2 });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("{Iterator.Name}{Iterator.Name}", actual);
    }

    [Fact]
    public void LoopValueWhenNotInLoop_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{Iterator}", "Iterator", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { new object() });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void LoopIterationWhenNotInLoop_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringTokenSegment("{::loopiteration}", "::loopiteration", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { new object() });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer));
    }

    [Fact]
    public void NestedLoopWithLoopIterationPseudo_OutputIterationOfInnerLoop()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "3"),
            new InterpolatedStringLiteralSegment("a"),
            new InterpolatedStringCommandSegment("{:loop}", "loop", string.Empty, "2"),
            new InterpolatedStringTokenSegment("{::loopiteration}", "::loopiteration", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
            new InterpolatedStringLiteralSegment("b"),
            new InterpolatedStringTokenSegment("{::loopiteration}", "::loopiteration", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("a12b1a12b2a12b3", actual);
    }

    [Fact]
    public void NestedLoopWithTwoSequences_OutputValuesFromEitherSequence()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "S1", string.Empty),
            new InterpolatedStringCommandSegment("{:loop}", "loop", "S2", string.Empty),
            new InterpolatedStringTokenSegment("{S1}", "S1", string.Empty, string.Empty),
            new InterpolatedStringTokenSegment("{S2}", "S2", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence1 = TokenValueContainerFactory.FromSequence(settings, "S1", new[] { "a", "b" });
        var sequence2 = TokenValueContainerFactory.FromSequence(settings, "S2", new[] { "c" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence1, sequence2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("acbc", actual);
    }

    [Fact]
    public void IfConditionUsingSequenceValue_SecondNameOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringCommandSegment("{:if}", "if", "Iterator.IsValue", string.Empty),
            new InterpolatedStringTokenSegment("{Iterator.Name}", "Iterator.Name", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:ifend}", "ifend", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var customSettings = StringTokenFormatterSettings.Default with
        {
            Commands = new[] { ExpanderCommandFactory.Conditional }.Concat(settings.Commands).ToList(),
        };
        var interpolatedString = new InterpolatedString(segments, customSettings);
        var o1 = TokenValueContainerFactory.FromObject(customSettings, new { IsValue = false, Name = "a" });
        var o2 = TokenValueContainerFactory.FromObject(customSettings, new { IsValue = true, Name = "b" });
        var sequence = TokenValueContainerFactory.FromSequence(customSettings, "Iterator", new[] { o1, o2 });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(customSettings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void LoopCount_CountOf2OutputTwice()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", string.Empty),
            new InterpolatedStringTokenSegment("{::loopcount}", "::loopcount", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { "a", "b" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("22", actual);
    }

    [Fact]
    public void LoopCountRespectsIterationsData_CountOf2OutputOnce()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringCommandSegment("{:loop}", "loop", "Iterator", "1"),
            new InterpolatedStringTokenSegment("{::loopcount}", "::loopcount", string.Empty, string.Empty),
            new InterpolatedStringCommandSegment("{:loopend}", "loopend", string.Empty, string.Empty),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { "a", "b" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("1", actual);
    }
}