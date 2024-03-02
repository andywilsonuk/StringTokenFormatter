# StringTokenFormatter v9.0

Provides token replacement for interpolated (templated) strings not known at compile time such as those retrieved from data stores (file system, database, API, config files etc) using a variety of token to value mappers.

It isn’t a replacement for string interpolation where the 'interpolation expression' is known at compile time and instead builds upon the idea of `string.Format` where a templated string is passed along with a series of replacement values. This library offers additional features beyond what either of the out-of-the-box implementations provide.

Supported platforms: .NET 5 and later, .NET Standard 2.0

Available on nuget.org at https://www.nuget.org/packages/StringTokenFormatter.

```C#
using StringTokenFormatter;

string interpolatedString = "Hello {FirstName} {LastName}";
var client = new {
    FirstName = "John",
    LastName = "Smith",
};
string message = interpolatedString.FormatFromObject(client);
```

# Migrating to version 9

See [the v8 migration page](/migration-v8.md) for details on breaking changes and how to upgrade from version 8 to version 9.

# Features overview

As well as `string` extensions, there are equivalent `Uri` extensions, the preferred method is to use the dependency injection friendly [Resolver](#interpolated-string-resolver) which makes sharing custom settings and working with complex templates easier.

```C#
string templateString = "Answer is {percent,10:P}";
var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);

string actual = resolver.FromTuples(templateString, ("percent", 1.2));

Assert.Equal("Answer is    120.00%", actual);
```

Tokens with formatting and alignment can be specified in the same way as `string.Format` ([.net docs](https://learn.microsoft.com/en-us/dotnet/api/system.string.format)). Alternative [token syntax](#syntax) can be selected in the [settings](#settings).

Grouping tokens with prefixes, multiple containers and other complex token resolution setups are supported through the `CompositeTokenValueContainer`, see [Building composite containers](#building-composite-token-value-containers) for the helper class.

Conditional blocks of text, loops and simple value mapping can be controlled through [commands](#commands).

As well supporting formatting through `IFormatProvider`, strongly-type `FormatterDefinitions` functions can be configured to match type, token name or format string.

[Value Containers](#value-containers) can return primatives (`string`, `int` etc) as well as `Func` or `Lazy` values which will be resolved before formatting using one of the [Value Converters](#valueconverters). Additional converters can be included to perform custom conversion logic after token matching but before formatting.

```C#
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

record OrderLine(string product, double price);
```


# Interpolated String Resolver

Once beyond simple use cases, things can get a little complicated and this is where the `InterpolatedStringResolver` steps in.

A helper class called `InterpolatedStringResolver` exists to allow the easy reuse of custom settings without overriding the global default. An IoC container could be used to store the resolver for use throughout the application. The resolver contains the standard expansion methods and is in some ways a preferred option to using the `string` extension methods.

The resolver provides methods for both expansion of tokens from `string` and parsed `InterpolatedString`.

# How token resolution works

To resolve the tokens within a string, there is a two stage process, first parsing, then expanding.

## Parsing

The `InterpolatedStringParser` turns a template `string` into an `InterpolatedString` defined as a list of strongly-typed `InterpolatedStringSegments`.

Generating the `InterpolatedString` takes time but can be stored and passed multiple times to the `InterpolatedStringExpander`. 

```mermaid
flowchart LR
    Settings[/settings/]
    TemplateString[/template string/]
    Parser[InterpolatedStringParser]
    InterpolatedStringSegments[/segments/]
    InterpolatedString[/InterpolatedString/]
    TemplateString --> Parser
    Settings --> Parser
    Parser --> InterpolatedStringSegments
    InterpolatedStringSegments --> InterpolatedString
```

## Expanding

The `InterpolatedStringExpander` take the `InterpolatedString` and processes it using the values in the `ValueContainer`.

1. The passed `ITokenValueContainer` provides the value based on the token name
2. A value conversion is then attempted based on the collection of `ValueConverters` in the settings
3. If the token contains alignment or formatting details, `string.Format` is called with the `FormatProvider` from the settings

Block Commands are processed by the `InterpolatedStringExpander` and follow the flow of steps 2.1 and 2.2 for obtaining their relevant values from tokens. 

```mermaid
flowchart LR
    InterpolatedString[/InterpolatedString/]
    ValueContainer[/ValueContainer/]
    Expander[InterpolatedStringExpander]
    Segment[/segment/]
    Command[Command]
    ValueConverter[Value Converter]
    FormatDefinition[Format Definition]
    FormatProvider[Format Provider]
    ResultantStringSegment[/resultant string/]
    ResultantString[/combined string/]
    subgraph SegmentProcessing[Segment Processing]
        direction TB
        Segment --> Command
        Command --> ValueConverter
        ValueConverter --> FormatDefinition
        ValueConverter --> FormatProvider
        FormatDefinition --> ResultantStringSegment
        FormatProvider --> ResultantStringSegment
    end
    InterpolatedString --> Expander
    ValueContainer --> Expander
    Expander --> SegmentProcessing
    SegmentProcessing --> ResultantString
```

# Value containers

Using properties of an object (including an anonymous object) to resolve tokens:
```C#
string original = "start {middle} end";
var tokenValues = new { Middle = "center" };
string result = original.FormatFromObject(tokenValues);
Assert.Equal("start center end", result);
```

Using a dictionary of values or other implementation of `IEnumerable<KeyValuePair<string, object>>` to resolve tokens:
```C#
string original = "start {middle} end";
var tokenValues = new Dictionary<string, object> { { "middle", "center" } };
string result = original.FormatFromPairs(tokenValues);
Assert.Equal("start center end", result);
```

Using an enumerable of `ValueTuples` to resolve tokens:
```C#
string original = "start {middle} end";
var tokenValues = new [] { ("middle", "center") };
string result = original.FormatFromTuples(source, tokenValues);
Assert.Equal("start center end", result);
```

Using a single name and value to resolve tokens:
```C#
string original = "start {middle} end";
string result = original.FormatFromSingle("middle", "center");
Assert.Equal("start center end", result);
```

Using a function to resolve tokens:
```C#
string original = "start {middle} end";
Func<string, object> func = (token) => { return "center"; };
string result = original.FormatFromFunc("middle", func);
Assert.Equal("start center end", result);
```

See [building composite token value containers](#building-composite-token-value-containers) for hierarchical or cascading containers. Also [custom containers](#creating-a-custom-itokenvaluecontainer).

Note: comma (,) and colon (:) should not be used in token names to avoid confusion with alignment and format values.

# Settings

All interpolating methods accept an optional `StringTokenFormatterSettings` parameter which is used in preference to the `StringTokenFormatterSettings.Global` settings.

The settings record is immutable so the `with` keyword is used to mutate the settings, so for example to replace the global settings, something like the following can be used:

```C#
StringTokenFormatterSettings.Global = StringTokenFormatterSettings.Global with { Syntax = CommonTokenSyntax.Round };
```

It should be noted that whilst overriding the global is a convenient action, it can cause side effects by other code using this library. Library implementations should not update `Global`. Alternately, consider creating an instance of `InterpolatedStringResolver` which takes the settings object in its constructor and provides the common methods for expanding from different `ITokenValueContainer` implementations.

## Creating Settings instances

Using the `Global` settings as the base:

```C#
var customSettings = StringTokenFormatterSettings.Global with { Syntax = CommonTokenSyntax.Round };
var expanded = "This interpolated string uses (token) as its syntax".FormatFromSingle("token", "expanded value", customSettings);
```

Using the default settings as the base:

```C#
var settings1 = new StringTokenFormatterSettings { Syntax = CommonTokenSyntax.Round };
// or
var settings2 = StringTokenFormatterSettings.Default with { Syntax = CommonTokenSyntax.Round };
```

Initially, the `Global` settings are the `Default` settings.

## Settings properties

### `Syntax`

Takes a `TokenSyntax` instance and defines the syntax is used for detecting tokens. Default `CommonTokenSyntax.Curly`.

Build-in syntax within the CommonTokenSyntax class:

| Name                   | Marker       | Escape |
| :--------------------: | :----------: | :----: |
| Curly                  | `{Token}`    | `{{`   |
| DollarCurly            | `${Token}`   | `${{`  |
| Round                  | `(Token)`    | `((`   |
| DollarRound            | `$(Token)`   | `$((`  |
| DollarRoundAlternative | `$(Token)`   | `$$(`  |

Note: Token markers are case sensitive.

### `FormatProvider`

Is used to specify the `IFormatProvider` applied to token values and uses [string.Format](https://learn.microsoft.com/en-us/dotnet/api/system.string.format) to apply formatting and alignment for example: `{value,10:D4}`. Default `CultureInfo.CurrentUICulture`.

### `NameComparer`

The comparer used by `ITokenValueContainer` when performing token to value look-ups. Takes a standard `StringComparer`. Default `StringComparer.OrdinalIgnoreCase`.

### `BlockCommands`

The collection of Block Commands to be used by the `InterpolatedStringExpander`. Default collection from `BlockCommandFactory`:

| Block              | Result                                                   |
| :----------------: | :------------------------------------------------------: |
| Conditional        | Controls includes of wrapped text based on boolean value |
| Loop               | Allows for repeated text based on specific iterations    | 

See [Block Commands](#block-commands) for more information.

### `TokenResolutionPolicy`

Controls how token values are handled by `ITokenValueContainer` implementations. Default `TokenResolutionPolicy.ResolveAll`.

The policies are:

| Policy            | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| ResolveAll        | Always uses the value returned                           |
| IgnoreNull        | Uses the value if it is not null                         |
| IgnoreNullOrEmpty | Uses the value if it is not null and not an empty string |

What happens next will depend upon what else is configured:

1. if this is a [`CompositeTokenValueContainer`](#building-composite-token-value-containers) then the matching will cascade to the next container
2. if `UnresolvedTokenBehavior` setting is set to `Throw` then an exception will be raised

### `UnresolvedTokenBehavior`

Defines what should happen if the token specified in the interpolated string cannot be matched within the `ITokenValueContainer`. Default `UnresolvedTokenBehavior.Throw`.

| Behavior          | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| Throw             | An `UnresolvedTokenException` exception is raised        |
| LeaveUnresolved   | The text will contain the original token unmodified      |

### `InvalidFormatBehavior`

Defines how string.Format exceptions are handled. Default `InvalidFormatBehavior.Throw`.

| Behavior          | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| Throw             | An `TokenValueFormatException` exception is raised       |
| LeaveUnformatted  | The text will contain the token value unformatted        |
| LeaveToken        | The text will contain the original token unmodified      |

### `ValueConverters`

Applies to token values after matching but before formatting. Converters are attempted in order so that once one has successfully converted the value then no further conversions take place. Default collection (from `TokenValueConverterFactory`):

| Value                       | Result                                                   |
| :-------------------------: | :------------------------------------------------------: |
| Null                        | no conversion                                            |
| `string` or `ValueType`     | no conversion                                            | 
| `Lazy<T>`                   | `Lazy.Value`                                             |
| `Func<T>`                   | function result                                          |
| `Func<string, T>`           | Supplied token name function result                      |

They can be useful to provide post-match functionality; a great example is a when using an object which contains a property that uses a `Lazy`. The token matcher resolves the token marker to property and then through the `ValueConverters` calls the `Lazy.Value` and returns the value of the `Lazy` for formatting.

All passed through types must be handled by a Value Converter otherwise an exception is thrown.

By design `ToString` is not used as a value converter because calling that on a custom object will yield the `object.ToString` result which is often not the intended behavior. For scenarios when the `ToString` has been overridden, the result of calling `TokenValueConverterFactory.ToStringConverter<T>` can be added to the settings list of `ValueConverters` so that the `ToString` method for that specific type will be used as a valid value conversion.

### `HierarchicalDelimiter`

Defines the prefix for `HierarchicalTokenValueContainer` instances. Default `.` (period).

See also [Token Value Container Builder](#building-composite-token-value-containers).

## Additional features and notes

### Building composite token value containers

The `TokenValueContainerBuilder` provides methods for creating `CompositeTokenValueContainer` instances.

Note that matching attempts are made in the order that the containers are passed to the `CompositeTokenValueContainer` instance which will be the same as the order that they are added to the builder. This includes nested containers.

Nested containers are supported such that `{prefix.token}` first matches the prefix and then uses the associated container to match the suffix. In the example below, the prefix is `Account` and the suffix `Id` exists as a property on the `account` object.

```C#
var account = new {
    Id = 2,
    Name = "The second account",
};

var builder = new TokenValueContainerBuilder(StringTokenFormatterSettings.Default);
builder.AddSingle("text", "Message text");
builder.AddNestedObject("Account", account);
var combinedContainer = builder.CombinedResult();

string interpolatedString = "Ref: {Account.Id}. {text}.";
string actual = interpolatedString.FormatFromContainer(combinedContainer);

Assert.Equal("Ref: 2. Message text.", actual);
```

The delimiter can changed in the [settings](#hierarchicaldelimiter).

Deep nesting is supported but discouraged, instead opt for flatter composites by adding the nested container to the top level with a separate prefix.

### Block commands

Introduced in v8, Block Commands wrap literal or token segments and provide behavior configurable by container values.

The block command names are always lowercase whilst tokens abide by the `TokenResolutionPolicy` (as well as `ValueConverters`).

#### Conditional block

Simple boolean conditions can be used to exclude blocks of text. 

The starting command format is `:if,token` where the `token` resolves to a boolean value dictating whether to include the block. The ending command is `:ifend` and can optionally include the token name.

```C#
string original = "start {:if,IsValid}{middle}{:ifend,IsValid} end";
var tokenValues = new { Middle = "center", IsValid = false };
string result = original.FormatFromObject(tokenValues);
Assert.Equal("start  end", result);
```

Nested conditions are supported.

#### Loop blocks

Provides a fixed number of iterations for the nested block.

For token derived iterations `{:loop,token}` is used and for constant iterations `{:loop:value}` where `value` is the number of iterations required. The ending command is `{:loopend}`.

```C#
string original = "outside {:loop,Iterations}{innerValue}{:loopend} outside";
int callCount = 0;
var called = () => {
    callCount++;
    return callCount switch
    {
        1 => "a",
        2 => "b",                
        _ => throw new IndexOutOfRangeException("Too many calls"),
    };
};
var tokenValues = new { Iterations = 2, InnerValue = called };
string result = original.FormatFromObject(tokenValues);
Assert.Equal("outside ab outside", result);
```

In this example, the token value `InnerValue` is a `Func<string>` which returns a different value on each call to the function.

Nested loops are supported.

### Creating a custom `ITokenValueContainer`

Whilst there are a number of built-in containers, it many be necessary to create a complete custom container. The container should take in the settings interface `ITokenValueContainerSettings` and obey `NameComparer` and `TokenResolutionPolicy` properties. 

See also [Token Value Container Builder](#building-composite-token-value-containers).

### `FrozenDictionary` support in .NET 8

When compiling against the .NET 8 version of the library, a method `Frozen` is available on `DictionaryTokenValueContainer` and `ObjectTokenValueContainer` instances which sets the backing dictionary and can provide a significant performance boost when reusing instances.

### Async loading of token values

There is no plan to support async/await within the library, the reason is that the library is designed to the CPU-bound and adding in an IO-bound layer massively changes the design and considered use-cases.

The `InterpolatedString` returned by the `InterpolatedStringParser` contains an extension method `Tokens` which provides a unique list of tokens found within the interpolated string. These token names can be used by an async method to, for example, request the token values from a data store. The token values can be loaded into an object or `IEnumerable<KeyValuePair<string, T>>` and provided as a parameter to the matching `TokenValueContainerFactory` method. The `InterpolatedString` and `ITokenValueContainer` can then be passed to the `InterpolatedStringExpander.Expand` method which in turns returns the resultant string.
