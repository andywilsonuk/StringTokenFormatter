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
    public void AddSequence_MultipleContainerCombinations_AllUsed()
    {
        var account = new
        {
            Id = 2,
        };
        Dictionary<string, string> person = new()
        {
            { "Name", "Eric" },
        };

        builder.AddSingle("text", "message text");
        builder.AddPrefixedObject("Account", account);
        builder.AddPrefixedKeyValues("Person", person);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        string interpolatedString = "Hi {Person.Name}, {text}. Ref: {Account.Id}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Hi Eric, message text. Ref: 2", actual);
    }

    [Fact]
    public void AddPrefixedObject_NestedProperties_WhenEnabled_AreResolved()
    {
        StringTokenFormatterSettings nestedSettings = settings with { ResolveNestedValues = NestedValueResolution.Nested };
        InterpolatedStringResolver nestedResolver = new(nestedSettings);
        var account = new { Customer = new { Name = "Ada" } };
        ITokenValueContainer combinedContainer = nestedResolver.Builder()
            .AddPrefixedObject("Account", account)
            .CombinedResult();

        string actual = nestedResolver.FromContainer("{Account.Customer.Name}", combinedContainer);

        Assert.Equal("Ada", actual);
    }

    [Fact]
    public void AddSequence_StringSequence_PrimativeValuesMaintained()
    {
        builder.AddSequence("Seq", ["first", "second"]);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        SequenceTokenValueContainer container = Assert.IsAssignableFrom<SequenceTokenValueContainer>(combinedContainer.TryMap("Seq").Value);
        TryGetResult actual = container.TryMap("Seq", 1);

        Assert.True(actual.IsSuccess);
        Assert.IsAssignableFrom<string>(actual.Value);
    }

    [Fact]
    public void AddSequence_IntSequence_PrimativeValuesMaintained()
    {
        builder.AddSequence("Seq", [1, 2]);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        SequenceTokenValueContainer container = Assert.IsAssignableFrom<SequenceTokenValueContainer>(combinedContainer.TryMap("Seq").Value);
        TryGetResult actual = container.TryMap("Seq", 1);

        Assert.True(actual.IsSuccess);
        Assert.IsAssignableFrom<int>(actual.Value);
    }

    [Fact]
    public void AddSequence_ClassSequence_ObjectValueOutput()
    {
        builder.AddSequence("Seq", [new ComplexClass("first"), new ComplexClass("second")]);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Seq}{Seq.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }

    [Fact]
    public void AddSequence_StructSequence_ObjectValueOutput()
    {
        builder.AddSequence("Seq", [new ComplexStruct("first"), new ComplexStruct("second")]);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Seq}{Seq.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }

    [Fact]
    public void AddSequence_AnonymousSequence_ObjectValueOutput()
    {
        builder.AddSequence("Seq", [new { Value = "first" }, new { Value = "second" }]);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        string interpolatedString = "{:loop,Seq}{Seq.Value} {:loopend}";
        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("first second ", actual);
    }

    [Fact]
    public void AddPrefixedSequence_StringSequence_PrimativeValuesMaintained()
    {
        builder.AddPrefixedSequence("prefix", "Seq", ["first", "second"]);
        ITokenValueContainer combinedContainer = builder.CombinedResult();

        TryGetResult actual = combinedContainer.TryMap("prefix.Seq");

        Assert.True(actual.IsSuccess);
        Assert.IsAssignableFrom<SequenceTokenValueContainer>(actual.Value);
    }
}
