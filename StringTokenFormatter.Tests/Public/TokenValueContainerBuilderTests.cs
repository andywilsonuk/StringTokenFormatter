namespace StringTokenFormatter.Tests;

public class TokenValueContainerBuilderTests
{
    [Fact]
    public void MultipleContainerCombinations()
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
}
