using System.Text;

namespace StringTokenFormatter.Tests;

public class Examples
{
    /// <summary>
    /// Anonymous types can be used as containers for values.
    /// Using `string` extensions.
    /// </summary>
    [Fact]
    public void ObjectContainerFromAnonymousType()
    {
        string interpolatedString = "Hello {FirstName} {LastName}";
        var client = new
        {
            FirstName = "John",
            LastName = "Smith",
        };

        string actual = interpolatedString.FormatFromObject(client);

        Assert.Equal("Hello John Smith", actual);
    }

    /// <summary>
    /// Alignment and Format have feature parity with `string.Format`.
    /// Using `Resolver` with `Tuples key/value pairs.
    /// </summary>
    [Fact]
    public void FormattingParityWithStringFormat()
    {
        string interpolatedString = "Today is {outlook} with a temperature of {temperature:N1}°C and {humidity,6:P} humidity";
        var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);
        var tokenValues = new (string, object)[] { ("temperature", 23), ("humidity", 0.6m), ("outlook", "sunny") };

        string actual = resolver.FromTuples(interpolatedString, tokenValues);

        Assert.Equal("Today is sunny with a temperature of 23.0°C and 60.00% humidity", actual);
    }

    /// <summary>
    /// Loop commands can use list of primatives, in this case `string`.
    /// Map commands can use `Func` expressions.
    /// Container builder for ease of use.
    /// </summary>
    [Fact]
    public void LoopWithPrimativeValuesList_MapCommandUsingFunc()
    {
        string interpolatedString = new StringBuilder()
            .Append("<table>")
            .Append("{:loop,ListValue}<tr class=\"{:map,IsEven:false=stripe,true=no-stripe}\">")
            .Append("<td>{::loopiteration:D2}/{::loopcount:D2}</td>")
            .Append("<td>{ListValue}</td>")
            .Append("</tr>{:loopend}")
            .Append("</table>")
            .ToString();
        int rowIndex = 0;
        var listValues = new List<string> { "Apple", "Banana", "Cherry", "Damson", "Elderberry" };
        var tokenValues = new Dictionary<string, object>()
        {
            ["IsEven"] = () => rowIndex++ % 2 == 1,
        };

        var combinedContainer = new TokenValueContainerBuilder(StringTokenFormatterSettings.Default)
            .AddKeyValues(tokenValues)
            .AddSequence("ListValue", listValues)
            .CombinedResult();

        string actual = interpolatedString.FormatFromContainer(combinedContainer);

        string expected = new StringBuilder()
            .Append("<table>")
            .Append("<tr class=\"stripe\"><td>01/05</td><td>Apple</td></tr>")
            .Append("<tr class=\"no-stripe\"><td>02/05</td><td>Banana</td></tr>")
            .Append("<tr class=\"stripe\"><td>03/05</td><td>Cherry</td></tr>")
            .Append("<tr class=\"no-stripe\"><td>04/05</td><td>Damson</td></tr>")
            .Append("<tr class=\"stripe\"><td>05/05</td><td>Elderberry</td></tr>")
            .Append("</table>")
            .ToString();
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Format definitions by type or by type and token name.
    /// Container builder instance from `Resolver`.
    /// </summary>
    [Fact]
    public void CustomFormatDefinitions()
    {
        static string intFormatter(int value, string formatString) => value.ToString("D3");
        static string nameFormatter(string value, string formatString) => formatString == "titleCase" ? $"{value.Substring(0, 1).ToUpper()}{value.Substring(1).ToLower()}" : value;
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatterDefinitions = new[] {
                FormatterDefinition.ForType<int>(intFormatter),
                FormatterDefinition.ForTokenName<string>("Account.Name", nameFormatter)
            },
        };
        var resolver = new InterpolatedStringResolver(settings);
        var account = new
        {
            Id = 2,
            Name = "Savings Account",
        };
        var combinedContainer = resolver.Builder()
            .AddNestedObject("Account", account)
            .AddSingle("text", "It has come to our attention that...")
            .CombinedResult();
        var interpolatedString = resolver.Interpolate("Ref: {Account.Id}, {Account.Name:titleCase}. {text}");

        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Ref: 002, Savings account. It has come to our attention that...", actual);
    }

    /// <summary>
    /// Conditional command plus negation. Map command using enum.
    /// </summary>
    private enum ModeOfTransport { Unknown = 0, Bike = 1, Car = 2, Bus = 3, }
    [Fact]
    public void ConditionalCommand_MapCommand()
    {
        var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);
        string interpolatedString = new StringBuilder()
            .Append("{:if,travelledToWork}{:map,mode:Unknown=Not set,Bike=Self propelled,Car=Combustion engine,Bus=Electric}{:ifend}")
            .Append("{:if,!travelledToWork}Did not travel{:ifend}")
            .ToString();
        var combinedContainer = resolver.Builder()
            .AddSingle("travelledToWork", true)
            .AddSingle("mode", ModeOfTransport.Bike)
            .CombinedResult();

        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Self propelled", actual);
    }

    /// <summary>
    /// A more complex example using numerous features.
    /// </summary>
    private class OrderLine(string product, int quantity, double price)
    {
        public string Product { get; } = product;
        public int Quantity { get; } = quantity;
        public double Price { get; } = price;
    }
    [Fact(Skip = "incomplete")]
    public void ComplexExample()
    {
        var random = new Random();
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatProvider = CultureInfo.GetCultureInfo("en-GB"),
        };
        var resolver = new InterpolatedStringResolver(settings);
        string interpolatedStringRaw = new StringBuilder()
            .Append("")
            .ToString();
        var interpolatedString = resolver.Interpolate(interpolatedStringRaw);

        var customer = new
        {
            Name = "James Strong",
            IsFirstOrder = true,
        };
        var order = new Dictionary<string, object>()
        {
            ["Id"] = "#8321",
            ["PaymentMethod"] = "Credit card",
            ["Delivery"] = "Next day",
            ["DeliveryComment"] = "Please leave if no one in",
        };
        var orderLines = new[]
        {
            new OrderLine(product: "T-shirt size M", quantity: 2, price: 25.50),
            new OrderLine(product: "Coat size L", quantity: 1, price: 40.0),
            new OrderLine(product: "Socks one size", quantity: 4, price: 14.0),
        };
        var combinedContainer = resolver.Builder()
            .AddNestedObject("Customer", customer)
            .AddNestedKeyValues("Order", order)
            .AddSequence("OrderLines", orderLines)
            .AddSingle("OrderTotal", orderLines.Sum(x => x.Price))
            .AddSingle("UniqueId", () => random.Next())
            .CombinedResult();

        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("", actual);
    }
}