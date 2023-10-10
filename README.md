# StringTokenFormatter v7.0
A high-speed library to parse interpolated strings at runtime and replace tokens with corresponding values.

```
var client = new {
    FirstName = "John",
    LastName = "Smith",
};

var message = "Hello {FirstName} {LastName}".FormatFromObject(client);
```

Available on NuGet at https://www.nuget.org/packages/StringTokenFormatter/

To get started, include the `using` statement so that the `string` extension methods are available:
```C#
using StringTokenFormatter;
```

Tokens with formatting and alignment can be specified in the same way as [string.format](https://learn.microsoft.com/en-us/dotnet/api/system.string.format), for example: `{value,10:D4}`.

# Supported .NET versions
- v7.0: .NET 6, .net framework 4.8  
- v6.1: .NET 6, .net framework 4.8 
- v6.0 and earlier: .NET Standard 2.0, .NET Framework 4.0

# Migrating from version 6
There are major breaking changes. See [the v6 migration page](/migration-v6.md) for details on how to upgrade from version 6 to version 7.

# Example usages

Using an object's properties to resolve tokens:
```C#
string original = "start {middle} end";
var tokenValues = new { Middle = "center" };
string result = original.FormatFromObject(tokenValues);
Assert.Equal("start center end", result);
```

Using a dictionary of values to resolve tokens:
```C#
string original = "start {middle} end";
var tokenValues = new Dictionary<string, object> { { "middle", "center" } };
string result = original.FormatFromPairs(tokenValues);
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

Replacing tokens within a URI:
``` C#
Uri original = new Uri("http://temp.org/{endpoint}?id={id}");
var tokenValues = new Dictionary<string, object> 
{ 
  { "endpoint", "people" },
  { "id", 10 }
};

Uri actual = original.FormatFromPairs(tokenValues);

Assert.Equal(new Uri("http://temp.org/people?id=10"), actual);
```

# Settings
All interpolating methods accept an optional `StringTokenFormatterSettings` parameter which is used in preference to the `StringTokenFormatterSettings.Global` settings.

The settings record is immutable so the `with` keyword is used to mutate the settings, so for example to replace the global settings, something like the following can be used:

```C#
StringTokenFormatterSettings.Global = StringTokenFormatterSettings.Global with { Syntax = CommonTokenSyntax.Round };
```

It should be noted that whilst overriding the global is a convenient action, it can cause side effects by other code using this library. Library implementations should not update `Global`. Alternately, consider creating an instance of `InterpolatedStringResolver` which takes the settings object in its constructor and provides the common methods for expanding from different `ITokenValueContainer` implementations.

## Creating instances of the settings

Using the `Global` settings as the base:

```C#
var settings1 = StringTokenFormatterSettings.Global with { Syntax = CommonTokenSyntax.Round };
var expanded = "This interpolated string uses (token) as its syntax".FormatFromSingle("token", "expanded value", settings1);
```

Using the default settings as the base:

```C#
var settings2 = new StringTokenFormatterSettings { Syntax = CommonTokenSyntax.Round };
var expanded = "This interpolated string uses (token) as its syntax".FormatFromSingle("token", "expanded value", settings2);
```

Initially, the `Global` settings are the default settings.

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

Is used to specify the `IFormatProvider` applied to token values and uses [string.format](https://learn.microsoft.com/en-us/dotnet/api/system.string.format) to apply formatting and alignment for example: `{value,10:D4}`. Default `CultureInfo.CurrentUICulture`.

### NameComparer

The comparer used by `ITokenValueContainer` when performing token to value look-ups. Takes a standard `StringComparer`. Default `StringComparer.OrdinalIgnoreCase`.

### TokenResolutionPolicy

Controls how token values are handled by `ITokenValueContainer` implementations. Default `TokenResolutionPolicy.ResolveAll`.

The policies are:

| Policy            | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| ResolveAll        | Always uses the value returned                           |
| IgnoreNull        | Uses the value if it is not null                         |
| IgnoreNullOrEmpty | Uses the value if it is not null and not an empty string |

What happens next will depend upon what else is configured:

1. if this is a [`CompositeTokenValueContainer`](#additional-features-and-notes) then the matching will cascade to the next container
2. if `UnresolvedTokenBehavior` setting is set to `Throw` then an exception will be raised

### UnresolvedTokenBehavior

Defines what should happen if the token specified in the interpolated string cannot be matched within the `ITokenValueContainer`. Default `UnresolvedTokenBehavior.Throw`.

| Behavior          | Result                                                   |
| :---------------: | :------------------------------------------------------: |
| Throw             | An `UnresolvedTokenException` exception is raised        |
| LeaveUnresolved   | The text will contain the original token unmodified      |

### ValueConverters

Applies to token values after matched and before formatting. Converters are attempted in order so that once one has successfully converted the value then no further conversions take place. Default collection (from `TokenValueConverters`):

| Value                        | Result                                                   |
| :--------------------------: | :------------------------------------------------------: |
| Null                         | no conversion                                            |
| Primitive (string, int, etc) | no conversion                                            | 
| Lazy\<object>                | Lazy.Value                                               |
| Func\<object>                | function result                                          |
| Func\<string, object>        | function result                                          |

They can be useful to provide post-match functionality; a great example is a when using an object which contains a property that uses a `Lazy`. The token matcher resolves the token marker to property and then through the `ValueConverters` calls the `Lazy.Value` and returns the value of the `Lazy` for formatting. 

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

### Creating a custom `ITokenValueContainer`

Whilst there are a number of built-in containers, it many be necessary to create a complete custom container. The container should take in the settings interface `ITokenValueContainerSettings` and obey `NameComparer` and `TokenResolutionPolicy` properties. 

It is also possible to combine multiple containers using `TokenValueContainerFactory.FromCombination` so that containers of the same type or differing types can be used to match tokens to values. Note that matching attempts are made in the order that the containers are passed to the `CompositeTokenValueContainer` instance.

### Async loading of token values

There is no plan to support async/await within the library, the reason is that the library is designed to the CPU-bound and adding in an IO-bound layer massively changes the design and considered use-cases.

The `InterpolatedString` returned by the `InterpolatedStringParser` contains an extension method `Tokens` which provides a unique list of tokens found within the interpolated string. These token names can be used by an async method to, for example, request the token values from a data store. The token values can be loaded into an object or `IEnumerable<KeyValuePair<string, T>>` and provided as a parameter to the matching `TokenValueContainerFactory` method. The `InterpolatedString` and `ITokenValueContainer` can then be passed to the `InterpolatedStringExpander.Expand` method which in turns returns the resultant string.
