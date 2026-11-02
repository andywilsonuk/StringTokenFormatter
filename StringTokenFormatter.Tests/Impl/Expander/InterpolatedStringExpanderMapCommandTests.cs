namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderMapCommandTests
{
    private readonly BasicContainer valuesContainer = new();

    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderMapCommandTests()
    {
        settings = StringTokenFormatterSettings.Default with
        {
            Commands =
            [
                ExpanderCommandFactory.Map,
                ExpanderCommandFactory.Loop,
                ExpanderCommandFactory.Standard,
            ]
        };
    }

    private enum TestEnum { First = 0, Second = 1 }

    [Fact]
    public void MapEnumValue_SecondCaseValueOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "First=a,Second=b")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", TestEnum.Second);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapBoolValue_FalseCaseValueOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "true=a,false=b")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", false);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapStringValueCaseInsensitive_MappedValueOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "FIRST=a,SECOND=b")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", "second");

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapIntValue_Case3ValueOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a,2=b,3=c")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", 3);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("c", actual);
    }

    [Fact]
    public void NonMatchingMapValue_Throws()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", 0);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void NonMatchingMapValueWithLeaveUnformattedFlag_OutputsRaw()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveUnformatted,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void NonMatchingMapValueWithLeaveTokenFlag_OutputsRaw()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveToken,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:map,TestCase:1=a}", actual);
    }

    [Fact]
    public void MapIntUsingFunc_SecondCaseValueOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a,2=b")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", () => 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void EmptyMappedValue_OutputsEmpty()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=,2=b")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", 1);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void MapValue_SkipsMalformedEntriesAndPreservesEqualsInMappedValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "=ignored,broken,Last=left=right")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", "Last");

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("left=right", actual);
    }

    [Fact]
    public void MapValue_UsesInvariantCultureComparison()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "caf\u00E9=matched")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", "cafe\u0301");

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("matched", actual);
    }

    [Fact]
    public void EmptyMapWithLeaveUnformatted_OutputsConvertedValue()
    {
        StringTokenFormatterSettings customSettings = settings with
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveUnformatted,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, customSettings);
        valuesContainer.Add("TestCase", "unmapped");

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("unmapped", actual);
    }

    [Fact]
    public void MapDiscardValue_IsUsedOnlyWhenItIsLastValidEntry()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "_=fallback,Other=actual,broken")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", "Missing");

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void UnresolvedTokenWithSetting_OutputsRaw()
    {
        StringTokenFormatterSettings customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        InterpolatedString interpolatedString = new(segments, customSettings);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:map,TestCase:1=a}", actual);
    }

    [Fact]
    public void MapDiscardSpecifiedwithMissingCase_OutputDiscardValue()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a,_=c")
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", 2);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("c", actual);
    }

    [Fact]
    public void MapPrimativeInLoop_SecondMappedValueOutput3Times()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Command("map", "TestCase", "First=a,Second=b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        valuesContainer.Add("TestCase", TestEnum.Second);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("bbb", actual);
    }

    [Fact]
    public void MapLoopSimpleValue_FirstThenSecondOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "Iterator", "First=a,Second=b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [TestEnum.First, TestEnum.Second]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("ab", actual);
    }

    [Fact]
    public void MapLoopComplexValue_FirstThenSecondOutput()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "Iterator.Name", "First=a,Second=b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { Name = TestEnum.First });
        var o2 = TokenValueContainerFactory.FromObject(settings, new { Name = TestEnum.Second });
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", [o1, o2]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("ab", actual);
    }

    [Fact]
    public void MapLoopCurrentIteration_OutputEachMapValueOnce()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "::loopiteration", "1=first,2=second,_=other")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", ["a", "b", "c"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("firstsecondother", actual);
    }

    [Fact]
    public void MapLoopCount_OutputThird3TimesOnceForEachItem()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "::loopcount", "1=first,2=second,3=third")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        InterpolatedString interpolatedString = new(segments, settings);
        SequenceTokenValueContainer sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", ["a", "b", "c"]);
        CompositeTokenValueContainer wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        string actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("thirdthirdthird", actual);
    }

    [Fact]
    public void UnresolvedMapTokenWithLeaveUnresolved_OutputsRaw()
    {
        List<InterpolatedStringSegment> segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
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