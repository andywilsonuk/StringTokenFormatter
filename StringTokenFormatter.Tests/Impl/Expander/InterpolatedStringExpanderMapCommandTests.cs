namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderMapCommandTests
{
    private readonly BasicContainer valuesContainer = new();

    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderMapCommandTests()
    {
        settings = StringTokenFormatterSettings.Default with
        {
            Commands = new List<IExpanderCommand>
            {
                ExpanderCommandFactory.Map,
                ExpanderCommandFactory.Loop,
                ExpanderCommandFactory.Standard,
            }
        };
    }

    private enum TestEnum { First = 0, Second = 1 }

    [Fact]
    public void MapEnumValue_SecondCaseValueOutput()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "First=a,Second=b")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", TestEnum.Second);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapBoolValue_FalseCaseValueOutput()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "true=a,false=b")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapStringValueCaseInsensitive_MappedValueOutput()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "FIRST=a,SECOND=b")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", "second");

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapIntValue_Case3ValueOutput()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a,2=b,3=c")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("c", actual);
    }

    [Fact]
    public void NonMatchingMapValue_Throws()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 0);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void NonMatchingMapValueWithLeaveUnformattedFlag_OutputsRaw()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveUnformatted,
        };
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("2", actual);
    }

    [Fact]
    public void NonMatchingMapValueWithLeaveTokenFlag_OutputsRaw()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            InvalidFormatBehavior = InvalidFormatBehavior.LeaveToken,
        };
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:map,TestCase:1=a}", actual);
    }

    [Fact]
    public void MapIntUsingFunc_SecondCaseValueOutput()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a,2=b")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", () => 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void EmptyMappedValue_OutputsEmpty()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=,2=b")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 1);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void UnresolvedTokenWithSetting_OutputsRaw()
    {
        var customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        var interpolatedString = new InterpolatedString(segments, customSettings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:map,TestCase:1=a}", actual);
    }

    [Fact]
    public void MapDiscardSpecifiedwithMissingCase_OutputDiscardValue()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a,_=c")
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("c", actual);
    }

    [Fact]
    public void MapPrimativeInLoop_SecondMappedValueOutput3Times()
    {
        var segments = new SegmentBuilder()
            .Command("loop", string.Empty, "3")
            .Command("map", "TestCase", "First=a,Second=b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", TestEnum.Second);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("bbb", actual);
    }

    [Fact]
    public void MapLoopSimpleValue_FirstThenSecondOutput()
    {
        var segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "Iterator", "First=a,Second=b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { TestEnum.First, TestEnum.Second });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("ab", actual);
    }

    [Fact]
    public void MapLoopComplexValue_FirstThenSecondOutput()
    {
        var segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "Iterator.Name", "First=a,Second=b")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        var o1 = TokenValueContainerFactory.FromObject(settings, new { Name = TestEnum.First });
        var o2 = TokenValueContainerFactory.FromObject(settings, new { Name = TestEnum.Second });
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { o1, o2 });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("ab", actual);
    }

    [Fact]
    public void MapLoopCurrentIteration_OutputEachMapValueOnce()
    {
        var segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "::loopiteration", "1=first,2=second,_=other")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { "a", "b", "c" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("firstsecondother", actual);
    }

    [Fact]
    public void MapLoopCount_OutputThird3TimesOnceForEachItem()
    {
        var segments = new SegmentBuilder()
            .Command("loop", "Iterator", string.Empty)
            .Command("map", "::loopcount", "1=first,2=second,3=third")
            .Command("loopend", string.Empty, string.Empty)
            .Build();
        var interpolatedString = new InterpolatedString(segments, settings);
        var sequence = TokenValueContainerFactory.FromSequence(settings, "Iterator", new[] { "a", "b", "c" });
        var wrapperContainer = TokenValueContainerFactory.FromCombination(settings, sequence);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, wrapperContainer);

        Assert.Equal("thirdthirdthird", actual);
    }

    [Fact]
    public void UnresolvedMapTokenWithLeaveUnresolved_OutputsRaw()
    {
        var segments = new SegmentBuilder()
            .Command("map", "TestCase", "1=a")
            .Build();
        var customSettings = settings with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        var interpolatedString = new InterpolatedString(segments, customSettings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal(segments[0].Raw, actual);
    }
}