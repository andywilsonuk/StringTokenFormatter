namespace StringTokenFormatter.Tests;

public class InterpolatedStringExpanderMapCommandTests
{
    private readonly BasicContainer valuesContainer = new();

    private readonly StringTokenFormatterSettings settings;

    public InterpolatedStringExpanderMapCommandTests()
    {
        settings = StringTokenFormatterSettings.Default with {
            BlockCommands = new List<IBlockCommand>
            {
                BlockCommandFactory.Map,
            }
        };
    }

    private enum TestEnum { First = 0, Second = 1 }
    [Fact]
    public void MapEnumValue_SecondCaseValueOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:First=a,Second=b}", "map", "TestCase", "First=a,Second=b"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", TestEnum.Second);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapBoolValueCaseInsensitive_FalseCaseValueOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:true=a,false=b}", "map", "TestCase", "true=a,false=b"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", false);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void MapIntValueCaseInsensitive_Case3ValueOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:1=a,2=b,3=c}", "map", "TestCase", "1=a,2=b,3=c"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 3);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("c", actual);
    }

    [Fact]
    public void NonMatchingMapValue_Throws()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:1=a}", "map", "TestCase", "1=a"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", 0);

        Assert.Throws<ExpanderException>(() => InterpolatedStringExpander.Expand(interpolatedString, valuesContainer));
    }

    [Fact]
    public void MapIntUsingFunc_SecondCaseValueOutput()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:1=a,2=b}", "map", "TestCase", "1=a,2=b"),
        };
        var interpolatedString = new InterpolatedString(segments, settings);
        valuesContainer.Add("TestCase", () => (object)2);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("b", actual);
    }

    [Fact]
    public void EmptyMappedValue_OutputsEmpty()
    {
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:1=,2=b}", "map", "TestCase", "1=,2=b"),
        };
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
        var segments = new List<InterpolatedStringSegment>
        {
            new InterpolatedStringBlockSegment("{:map,TestCase:1=a}", "map", "TestCase", "1=a"),
        };
        var interpolatedString = new InterpolatedString(segments, customSettings);

        var actual = InterpolatedStringExpander.Expand(interpolatedString, valuesContainer);

        Assert.Equal("{:map,TestCase:1=a}", actual);
    }

    // value in loop
    // what about when UnresolvedTokenBehavior is LeaveUnresolved, this is a question for Conditional as well and actually loop as well
}