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

    [Fact]
    public void ConditionalLogic()
    {
        string original = "{:if,formalSalutation}Dear{:ifend}{:if,!formalSalutation}Hi{:ifend} {name}";
        var combinedContainer = new TokenValueContainerBuilder(StringTokenFormatterSettings.Default)
            .AddSingle("formalSalutation", false)
            .AddSingle("name", "Sally")
            .CombinedResult();

        string result = original.FormatFromContainer(combinedContainer);
        Assert.Equal("Hi Sally", result);
    }

    [Fact]
    public void CustomFormatDefinitions()
    {
        static string intFormatter(int value, string formatString) => value.ToString("D3");
        static string nameFormatter(string value, string formatString) => formatString == "titleCase" ? $"{value.Substring(0, 1).ToUpper()}{value.Substring(1).ToLower()}" : value;
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = new [] {
                FormatterDefinition.ForType<int>(intFormatter),
                FormatterDefinition.ForTokenName<string>("Account.Name", nameFormatter)
            },
        };
        var resolver = new InterpolatedStringResolver(settings);
        var account = new {
            Id = 2,
            Name = "The Second Account",
        };
        var combinedContainer = resolver.Builder()
            .AddSingle("text", "It has come to our attention that...")
            .AddNestedObject("Account", account)
            .CombinedResult();

        var interpolatedString = resolver.Interpolate("Ref: {Account.Id}, {Account.Name:titleCase}. {text}");

        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Ref: 002, The second account. It has come to our attention that...", actual);
    }

    private enum OrderType { Unknown = 0, Bike = 1, Car = 2, Bus = 3, }
    [Fact]
    public void ValueSelectionUsingMap()
    {        
        string interpolatedString = "{:map,OrderType:Unknown=Not set,Bike=Self propelled,Car=Combustion engine,Bus=Electric}";

        string actual = interpolatedString.FormatFromSingle("OrderType", OrderType.Bike);

        Assert.Equal("Self propelled", actual);
    }
    /*


we don't want to handle +M30 as this isn't about formatting. We need an example though of how to do this though! Custom container which accepts the token name+M30

nesting

more examples including loop, enum conversion (new ToString converter? Also custom).
    */
}