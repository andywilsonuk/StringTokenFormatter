namespace StringTokenFormatter.Tests;

public partial class TokenValueContainerBuilderTests
{
    private readonly StringTokenFormatterSettings settings;
    private readonly InterpolatedStringResolver resolver;
    private readonly TokenValueContainerBuilder builder;

    public TokenValueContainerBuilderTests()
    {
        settings = StringTokenFormatterSettings.Default;
        resolver = new(settings);
        builder = new TokenValueContainerBuilder(settings);
    }

    [Fact]
    public void MultipleContainerCombinations_AllUsed()
    {
        var account = new {
            Id = 2,
        };
        var person = new Dictionary<string, string>()
        {
            { "Name", "Eric" },
        };

        builder.AddSingle("text", "message text");
        builder.AddNestedObject("Account", account);
        builder.AddNestedKeyValues("Person", person);
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "Hi {Person.Name}, {text}. Ref: {Account.Id}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Hi Eric, message text. Ref: 2", actual);
    }

    [Fact]
    public void StringSequence_PrimativeValuesMaintained()
    {
        builder.AddSequence("Seq", new string[] { "first", "second" });
        var combinedContainer = builder.CombinedResult();

        var container = Assert.IsAssignableFrom<SequenceTokenValueContainer>(combinedContainer.TryMap("Seq").Value);
        var actual = container.TryMap("Seq", 1);

        Assert.True(actual.IsSuccess);
        Assert.IsAssignableFrom<string>(actual.Value);
    }

    [Fact]
    public void IntSequence_PrimativeValuesMaintained()
    {
        builder.AddSequence("Seq", new int[] { 1, 2 });
        var combinedContainer = builder.CombinedResult();

        var container = Assert.IsAssignableFrom<SequenceTokenValueContainer>(combinedContainer.TryMap("Seq").Value);
        var actual = container.TryMap("Seq", 1);

        Assert.True(actual.IsSuccess);
        Assert.IsAssignableFrom<int>(actual.Value);
    }

    [Fact]
    public void ClassSequence_ObjectValueOutput()
    {
        builder.AddSequence("Seq", new [] { new ComplexClass("first"), new ComplexClass("second") });
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Seq}{Seq.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }
    
    [Fact]
    public void StructSequence_ObjectValueOutput()
    {
        builder.AddSequence("Seq", new[] { new ComplexStruct("first"), new ComplexStruct("second") });
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Seq}{Seq.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }

    [Fact]
    public void AnonymousSequence_ObjectValueOutput()
    {
        builder.AddSequence("Seq", new[] { new { Value = "first" }, new { Value = "second" } });
        var combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Seq}{Seq.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }
}
