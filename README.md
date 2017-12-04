# StringTokenFormatter
Provides string extension methods for replacing tokens within strings (using the format '{name}') with their specified lookup value.

Available on NuGet at https://www.nuget.org/packages/StringTokenFormatter/

An ```IFormatProvider``` can be passed using the method overloads.

Tokens with formatting and alignment can be specified in the same way as string.Format for example: ```{token,10:D4}``` see this page on MSDN: http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#FormatItem

To get started include the ```using``` statement so that the extension methods are available:
```C#
using StringTokenFormatter;
```

## Usage 1: Passing a single token and value
```C#
string original = "start {middle} end";
string result = original.FormatToken("middle", "centre");
Assert.Equal("start centre end", result);
```
## Usage 2: Passing a token/value dictionary
```C#
string original = "start {middle} end";
var tokenValues = new Dictionary<string, object> { { "middle", "centre" } };
string result = original.FormatToken(tokenValues);
Assert.Equal("start centre end", result);
```
## Usage 3: Passing object with properties to be used as token dictionary
```C#
string original = "start {middle} end";
var tokenValues = new { Middle = "centre" };
string result = original.FormatToken(tokenValues);
Assert.Equal("start centre end", result);
```
## Usage 4a: Passing on-demand token value functions (for single or dictionary tokens)
```C#
string original = "start {middle} end";
Func<string, object> func = (token) => { return "centre"; };
string result = original.FormatToken("middle", func);
Assert.Equal("start centre end", result);
```
## Usage 4b:
```C#
string original = "start {middle} end";
Func<string, string> func = (token) => { return "centre"; };
string result = original.FormatToken("middle", func);
Assert.Equal("start centre end", result);
```
## Usage 4c:
```C#
string original = "start {middle} end";
Func<string> func = () => { return "centre"; };
string result = original.FormatToken("middle", func);
Assert.Equal("start centre end", result);
```
## Usage 5a: Passing lazy loading objects (for single or dictionary tokens)
```C#
string original = "start {middle} end";
Lazy<object> lazy = () => { return "centre"; };
string result = original.FormatToken("middle", lazy);
Assert.Equal("start centre end", result);
```
## Usage 5b:
```C#
string original = "start {middle} end";
Lazy<string> lazy = () => { return "centre"; };
string result = original.FormatToken("middle", lazy);
Assert.Equal("start centre end", result);
```
## Usage 6: Formatting tokens in URI objects 
``` C#
Uri original = new Uri("http://temp.org/{endpoint}?id={id}");
var tokenValues = new Dictionary<string, object> 
{ 
  { "endpoint", "people" },
  { "id", 10 }
};
Uri expected = new Uri("http://temp.org/people?id=10");

Uri actual = original.FormatToken(tokenValues);

Assert.Equal(expected, actual);
```
## Usage 7: Further customisation
The extension methods wrap most of the functionality provided by  class ```TokenReplacer``` however as of v2 more customisation can be achieved using the interfaces:
* ```ITokenMatcher``` - matches the tokens within the string
* ```ITokenToValueMapper``` - provides a implementation for converting a single token to a value (multiple can be passed to the constructor)
* ```IValueFormatter``` - provides the formatting of values to strings

Each of the interfaces has a default which is available as a static property on the TokenReplacer class.

Additionally the workflow of mapping input to output can be called directly using the ```MapTokens``` method which accepts the input string and an implementation of ```ITokenValueContainer``` which maps values to tokens.
