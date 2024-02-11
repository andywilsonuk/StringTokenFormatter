using System.Text;

namespace StringTokenFormatter.Tests;

public class Examples
{
    [Fact]
    public void ObjectContainerFromAnonymousType()
    {
        string interpolatedString = "Hello {FirstName} {LastName}";
        var client = new {
            FirstName = "John",
            LastName = "Smith",
        };

        string actual = interpolatedString.FormatFromObject(client);

        Assert.Equal("Hello John Smith", actual);
    }

    [Fact]
    public void FormattingParityWithStringFormat()
    {
        string interpolatedString = "Today is {outlook} with a temperature of {temperature:N1}°C and {humidity,6:P} humidity";
        var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);
        var tokenValues = new (string, object)[] { ("temperature", 23), ("humidity", 0.6m), ("outlook", "sunny") };

        string actual = resolver.FromTuples(interpolatedString, tokenValues);

        Assert.Equal("Today is sunny with a temperature of 23.0°C and 60.00% humidity", actual);
    }

    [Fact]
    public void LoopWithPrimativeValues()
    {
        string original = "<table>{:loop,ListValue}<tr class=\"{StripeClass}\"><td>{::loopiteration:D2}/{RowCount:D2}</td><td>{ListValue}</td></tr>{:loopend}</table>";
        int rowIndex = 0;
        var listValues = new List<string> { "Apple", "Banana", "Cherry", "Dragon Fruit", "Elderberry" };
        var tokenValues = new Dictionary<string, object>()
        {
            ["RowCount"] = listValues.Count,
            ["StripeClass"] = () => rowIndex++ % 2 == 0 ? "stripe" : "no-stripe",
        };

        var combinedContainer = new TokenValueContainerBuilder(StringTokenFormatterSettings.Default)
            .AddPairs(tokenValues)
            .AddSequence("ListValue", listValues)
            .CombinedResult();

        string actual = original.FormatFromContainer(combinedContainer);

        string expected = new StringBuilder()
            .Append("<table>")
            .Append("<tr class=\"stripe\"><td>01/05</td><td>Apple</td></tr>")
            .Append("<tr class=\"no-stripe\"><td>02/05</td><td>Banana</td></tr>")
            .Append("<tr class=\"stripe\"><td>03/05</td><td>Cherry</td></tr>")
            .Append("<tr class=\"no-stripe\"><td>04/05</td><td>Dragon Fruit</td></tr>")
            .Append("<tr class=\"stripe\"><td>05/05</td><td>Elderberry</td></tr>")
            .Append("</table>")
            .ToString();
        Assert.Equal(expected, actual);
    }

    // [Fact]
    // public void ObjectContainerFromAnonymousType()
    // {
    //     string original = "start {:if,IsValid}{middle}{:ifend,IsValid} end";
    //     var tokenValues = new { Middle = "center", IsValid = true };
    //     string result = original.FormatFromObject(tokenValues);
    //     Assert.Equal("start center end", result);
    // }

    // [Fact]
    // public void ObjectContainerFromAnonymousType()
    // {
    //     var account = new {
    //         Id = 2,
    //         Name = "The second account",
    //     };

    //     var builder = new TokenValueContainerBuilder(StringTokenFormatterSettings.Default);
    //     builder.AddSingle("text", "Message text");
    //     builder.AddNestedObject("Account", account);
    //     var combinedContainer = builder.CombinedResult();

    //     string interpolatedString = "Ref: {Account.Id}. {text}.";
    //     string actual = interpolatedString.FormatFromContainer(combinedContainer);

    //     Assert.Equal("Ref: 2. Message text.", actual);
    // }
    /*
loop with Func can be used for stripes

building an html document example

we don't want to handle +M30 as this isn't about formatting. We need an example though of how to do this though! Custom container which accepts the token name+M30

nesting

Also Func T value converter to provide template phrases such as token firstname converter to "Hi Sally". Need a better example

toupper example either as value converter or new format provider. The latter is best when the same token name or type might need different formatting such as upper, lower etc

more examples including loop, enum conversion (new ToString converter? Also custom).
    */
}