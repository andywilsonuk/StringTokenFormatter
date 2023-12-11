# StringTokenFormatter v7.3
This library provides token replacement for interpolated strings not known at compile time such as those retrieved from data stores (file system, database, API, config files etc) and offers support for a variety of token to value mappers.

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

# .NET versions
.NET 6, 7, 8 and .NET Standard 2.0 with C# 10 language features

# Migrating from version 6
There are major breaking changes. See [the v6 migration page](/migration-v6.md) for details on how to upgrade from version 6 to version 7.

# Features overview

As well as `string` extensions, there are equivalent `Uri` extensions and a reusable [Resolver](#the-resolver) class which allows for easier sharing of custom settings.

```C#
string source = "Answer is {percent,10:P}";
var resolver = new InterpolatedStringResolver(StringTokenFormatterSettings.Default);

string actual = resolver.FromTuples(source, ("percent", 1.2));

Assert.Equal("Answer is    120.00%", actual);
```

Tokens with formatting and alignment can be specified in the same way as `string.Format` ([.net docs](https://learn.microsoft.com/en-us/dotnet/api/system.string.format)). Alternative [token syntax](#syntax) can be selected in the settings.

Nested tokens (like `prefix.name`), cascading containers and other complex token resolution setups are supported through the `CompositeTokenValueContainer`, see [Building composite containers](#building-composite-token-value-containers) for the helper class.

Conditional blocks of text can be controlled through boolean token values and the [conditional syntax](#conditions-1), for example:

```C#
string original = "start {if:IsValid}{middle}{ifend:IsValid} end";
var tokenValues = new { Middle = "center", IsValid = true };
string result = original.FormatFromObject(tokenValues);
Assert.Equal("start center end", result);
```

The [Value Converter](#valueconverters) settings provide `Lazy` loading and function-resolved values and can be extended to perform custom conversion logic after token matching but before formatting. Any [value container](#value-containers) can return a `Lazy` or `Func` which will be resolved before formatting.

See also [additional features and notes](#additional-features-and-notes) for performance optimization strategies and advanced usage. 

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

### Syntax

Takes a `TokenSyntax` instance and defines the syntax is used for detecting tokens. Default `CommonTokenSyntax.Curly`.

Build-in syntax within the CommonTokenSyntax class:

| Name                   | Marker       | Escape |
| :--------------------: | :----------: | :----: |
| Curly                  | `{Token}`    | `{{`   |
| DollarCurly            | `${Token}`   | `${{`  |
| Round                  | `(Token)`    | `((`   |
| DollarRound            | `$(Token)`   | `$((`  |
| DollarRoundAlternative | `$(Token)`   | `$$(`  |

### FormatProvider

Is used to specify the `IFormatProvider` applied to token values and uses [string.Format](https://learn.microsoft.com/en-us/dotnet/api/system.string.format) to apply formatting and alignment for example: `{value,10:D4}`. Default `CultureInfo.CurrentUICulture`.

### NameComparer

The comparer used by `ITokenValueContainer` when performing token to value look-ups. Takes a standard `StringComparer`. Default `StringComparer.OrdinalIgnoreCase`.

### Conditions

Simple boolean conditions can be used to exclude blocks of text. `ConditionStartToken` with default `if:` signifies the start of the block whilst `ConditionEndToken` with default `ifend:` signifies the end. It is expected that after the condition prefix will be the name of the token whose boolean value dictates whether to include the block or not.

Nested conditions are supported.

### TokenResolutionPolicy

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

### UnresolvedTokenBehavior

Defines what should happen if the token specified in the interpolated string cannot be matched within the `ITokenValueContainer`. Default `UnresolvedTokenBehavior.Throw`.

| Behavior          | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| Throw             | An `UnresolvedTokenException` exception is raised        |
| LeaveUnresolved   | The text will contain the original token unmodified      |

### InvalidFormatBehavior

Defines how string.Format exceptions are handled. Default `InvalidFormatBehavior.Throw`.

| Behavior          | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| Throw             | An `TokenValueFormatException` exception is raised       |
| LeaveUnformatted  | The text will contain the token value unformatted        |
| LeaveToken        | The text will contain the original token unmodified      |

### ValueConverters

Applies to token values after matched and before formatting. Converters are attempted in order so that once one has successfully converted the value then no further conversions take place. Default collection (from `TokenValueConverters`):

| Value                       | Result                                                   |
| :-------------------------: | :------------------------------------------------------: |
| Null                        | no conversion                                            |
| `string` or `ValueType`     | no conversion                                            | 
| `Lazy<T>`                   | `Lazy.Value`                                             |
| `Func<T>`                   | function result                                          |
| `Func<string, T>`           | Supplied token name function result                      |

They can be useful to provide post-match functionality; a great example is a when using an object which contains a property that uses a `Lazy`. The token matcher resolves the token marker to property and then through the `ValueConverters` calls the `Lazy.Value` and returns the value of the `Lazy` for formatting.

All passed through types must be handled by a Value Converters otherwise an exception is thrown.

### HierarchicalDelimiter

Defines the prefix for `HierarchicalTokenValueContainer` instances. Default `.` (period).

See also [Token Value Container Builder](#building-composite-token-value-containers).

## Additional features and notes

### Flow of Control
When resolving the token values within an interpolated string, the following sequence is followed:

1. The `InterpolatedStringParser` turns a `string` into an `InterpolatedString`
2. The `InterpolatedStringExpander` take the `InterpolatedString` and processes it. For a given token
    1. The passed `ITokenValueContainer` provides the value based on the token name
    2. A value conversion is then attempted based on the collection of `ValueConverters` in the settings
    3. If the token contains alignment or formatting details, `string.Format` is called with the `FormatProvider` from the settings

### Reusing InterpolatedString instances

The `InterpolatedStringParser.Parse` method is responsible for identifying tokens within the source string and returning the `InterpolatedString` of segments. Generating the `InterpolatedString` takes time but can be stored and pass multiple times to the `InterpolatedStringExpander.Expand` method. 

An example would be a mail merge whereby the same message text is used but with client-specific details within the `ITokenValueContainer`.

See also [The Resolver](#the-resolver).

### The Resolver

A helper class called `InterpolatedStringResolver` exists to allow the easy reuse of custom settings without overriding the global default. An IoC container could be used to store the resolver for use throughout the application. The resolver contains the standard expansion methods and is in some ways a preferred option to using the `string` extension methods.

The resolver provides methods for both expansion of tokens from `string` and parsed `InterpolatedString`.

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

### Conditions

Simple boolean conditions can be used to exclude blocks of text. The `ITokenValueContainerSettings` contains the special token prefixes `ConditionStartToken` (default `if:`) and `ConditionEndToken` (default `ifend:`).

It is expected that after the condition prefix will be the name of the token whose boolean value dictates whether to include the block or not.

```C#
string original = "start {if:IsValid}{middle}{ifend:IsValid} end";
var tokenValues = new { Middle = "center", IsValid = true };
string result = original.FormatFromObject(tokenValues);
Assert.Equal("start center end", result);
```

Nested conditions are supported. For `ConditionEndToken`, the token name suffix is purely for clarity and is optional.

The condition prefixes are case sensitive whilst the token providing the boolean value abides by the `TokenResolutionPolicy` (as well as `ValueConverters`).

### Creating a custom `ITokenValueContainer`

Whilst there are a number of built-in containers, it many be necessary to create a complete custom container. The container should take in the settings interface `ITokenValueContainerSettings` and obey `NameComparer` and `TokenResolutionPolicy` properties. 

See also [Token Value Container Builder](#building-composite-token-value-containers).

### Async loading of token values

There is no plan to support async/await within the library, the reason is that the library is designed to the CPU-bound and adding in an IO-bound layer massively changes the design and considered use-cases.

The `InterpolatedString` returned by the `InterpolatedStringParser` contains an extension method `Tokens` which provides a unique list of tokens found within the interpolated string. These token names can be used by an async method to, for example, request the token values from a data store. The token values can be loaded into an object or `IEnumerable<KeyValuePair<string, T>>` and provided as a parameter to the matching `TokenValueContainerFactory` method. The `InterpolatedString` and `ITokenValueContainer` can then be passed to the `InterpolatedStringExpander.Expand` method which in turns returns the resultant string.
