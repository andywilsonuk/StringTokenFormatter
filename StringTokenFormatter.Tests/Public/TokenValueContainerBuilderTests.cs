namespace StringTokenFormatter.Tests;

public class TokenValueContainerBuilderTests
{
    [Fact]
    public void MultipleContainerCombinations_AllUsed()
    {
        var settings = StringTokenFormatterSettings.Default;
        var resolver = new InterpolatedStringResolver(settings);
        var builder = new TokenValueContainerBuilder(settings);

        var account = new {
            Id = 2,
        };
        var person = new Dictionary<string, string>()
        {
            { "Name", "Human" },
        };

        builder.AddSingle("text", "message text");
        builder.AddNestedObject("Account", account);
        builder.AddNestedPairs("Person", person);
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "Hi {Person.Name}, {text}. Ref: {Account.Id}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Hi Human, message text. Ref: 2", actual);
    }

    [Fact]
    public void StringSequence_PrimativeValueOutput()
    {
        var settings = StringTokenFormatterSettings.Default;
        var resolver = new InterpolatedStringResolver(settings);
        var builder = new TokenValueContainerBuilder(settings);

        builder.AddSequence("Iterator", "first", "second");
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Iterator}{Iterator} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }

    [Fact]
    public void IntSequence_PrimativeValueOutput()
    {
        var settings = StringTokenFormatterSettings.Default;
        var resolver = new InterpolatedStringResolver(settings);
        var builder = new TokenValueContainerBuilder(settings);

        builder.AddSequence("Iterator", 1, 2);
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Iterator}{Iterator} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("1 2 ", actual);
    }

    private record TestRecord(string Value);

    [Fact]
    public void RecordSequence_PrimativeValueOutput()
    {
        var settings = StringTokenFormatterSettings.Default;
        var resolver = new InterpolatedStringResolver(settings);
        var builder = new TokenValueContainerBuilder(settings);

        builder.AddSequence("Iterator", new TestRecord("first"), new TestRecord("second"));
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Iterator}{Iterator.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }

    [Fact]
    public void AnonymousSequence_PrimativeValueOutput()
    {
        var settings = StringTokenFormatterSettings.Default;
        var resolver = new InterpolatedStringResolver(settings);
        var builder = new TokenValueContainerBuilder(settings);

        builder.AddSequence("Iterator", new { Value = "first" }, new { Value = "second" });
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Iterator}{Iterator.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }
}
