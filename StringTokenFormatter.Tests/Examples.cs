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
        string templateString = "Hello {FirstName} {LastName}";
        var client = new
        {
            FirstName = "John",
            LastName = "Smith",
        };

        string actual = templateString.FormatFromObject(client);

        Assert.Equal("Hello John Smith", actual);
    }

    /// <summary>
    /// Alignment and Format have feature parity with `string.Format`.
    /// Using `Resolver` with `Tuples key/value pairs.
    /// </summary>
    [Fact]
    public void FormattingParityWithStringFormat()
    {
        string templateString = "Today is {outlook} with a temperature of {temperature:N1}°C and {humidity,6:P} humidity";
        var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);
        var tokenValues = new (string, object)[] { ("temperature", 23), ("humidity", 0.6m), ("outlook", "sunny") };

        string actual = resolver.FromTuples(templateString, tokenValues);

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
        string templateString = new StringBuilder()
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

        string actual = templateString.FormatFromContainer(combinedContainer);

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
            .AddPrefixedObject("Account", account)
            .AddSingle("text", "It has come to our attention that...")
            .CombinedResult();
        var interpolatedString = resolver.Interpolate("Ref: {Account.Id}, {Account.Name:titleCase}. {text}");

        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        Assert.Equal("Ref: 002, Savings account. It has come to our attention that...", actual);
    }

    /// <summary>
    /// Conditional command plus negation. Map command using enum.
    /// </summary>
    [Fact]
    public void ConditionalCommand_MapCommand()
    {
        var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);
        string templateString = new StringBuilder()
            .Append("{:if,travelledToWork}{:map,mode:Unknown=Not set,Bike=Self propelled,Car=Combustion engine,Bus=Electric}{:ifend}")
            .Append("{:if,!travelledToWork}Did not travel{:ifend}")
            .ToString();
        var combinedContainer = resolver.Builder()
            .AddSingle("travelledToWork", true)
            .AddSingle("mode", ModeOfTransport.Bike)
            .CombinedResult();

        string actual = resolver.FromContainer(templateString, combinedContainer);

        Assert.Equal("Self propelled", actual);
    }
    private enum ModeOfTransport { Unknown = 0, Bike = 1, Car = 2, Bus = 3, }

    /// <summary>
    /// A more complex example using numerous features.
    /// </summary>
    [Fact]
    public void OrderConfirmation()
    {
        var settings = StringTokenFormatterSettings.Default with
        {
            FormatProvider = CultureInfo.GetCultureInfo("en-US"),
            FormatterDefinitions = new[] {
                FormatterDefinition.ForTokenName<int>("Order.Id", (id, _format) =>  $"#{id:000000}"),
                FormatterDefinition.ForType<Guid>((guid, format) => format == "Initial" ? guid.ToString("D").Split('-')[0].ToUpperInvariant() : guid.ToString()),
            },
        };
        var resolver = new InterpolatedStringResolver(settings);
        string templateString = new StringBuilder()
            .AppendLine("Hi {Customer.Name},")
            .AppendLine("Thank you for {:map,Customer.IsFirstOrder:true=your first order,false=your order}.")
            .AppendLine("Order details")
            .AppendLine("- Id: {Order.Id}")
            .AppendLine("- Payment method: {:map,Order.PaymentMethod:DebitCard=Debit card,CreditCard=Credit card}")
            .AppendLine("- Delivery option: {Order.Delivery}")
            .AppendLine("{:if,Order.HasDeliveryComment}- Comment for delivery driver: {Order.DeliveryComment}{:ifend}")
            .AppendLine("Items")
            .Append("{:loop,OrderLines}")
            .AppendLine("- {OrderLines.Product} @ {OrderLines.Price:C}")
            .Append("{:loopend}")
            .AppendLine("Total: {OrderTotal:C}")
            .Append("Ref: {MessageId:Initial}")
            .ToString();
        var interpolatedString = resolver.Interpolate(templateString);

        var customer = new
        {
            Name = "Jane Strong",
            IsFirstOrder = true,
        };
        var order = new Dictionary<string, object>()
        {
            ["Id"] = 8321,
            ["PaymentMethod"] = "CreditCard",
            ["Delivery"] = "Next day",
            ["DeliveryComment"] = "Please leave if no one in",
        };
        var orderLines = new[]
        {
            new OrderLine(product: "T-shirt", price: 25.5),
            new OrderLine(product: "Coat", price: 40.0),
            new OrderLine(product: "Socks", price: 14.0),
        };
        var combinedContainer = resolver.Builder()
            .AddPrefixedObject("Customer", customer)
            .AddPrefixedKeyValues("Order", order)
            .AddPrefixedSingle("Order", "HasDeliveryComment", order["DeliveryComment"] is not null)
            .AddSequence("OrderLines", orderLines)
            .AddSingle("OrderTotal", orderLines.Sum(x => x.Price))
            .AddSingle("MessageId", new Lazy<object>(() => Guid.Parse("73054fad-ba31-4cc2-a1c1-ac534adc9b45")))
            .CombinedResult();

        string actual = resolver.FromContainer(interpolatedString, combinedContainer);

        string expected = """
        Hi Jane Strong,
        Thank you for your first order.
        Order details
        - Id: #008321
        - Payment method: Credit card
        - Delivery option: Next day
        - Comment for delivery driver: Please leave if no one in
        Items
        - T-shirt @ $25.50
        - Coat @ $40.00
        - Socks @ $14.00
        Total: $79.50
        Ref: 73054FAD
        """;
        Assert.Equal(expected, actual);
    }
    private class OrderLine(string product, double price)
    {
        public string Product { get; } = product;
        public double Price { get; } = price;
    }
}