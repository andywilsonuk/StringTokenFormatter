#StringTokenFormatter
Provides string extension methods for replacing tokens within strings (using the format '{name}') with their specified lookup value.

Available on NuGet at https://www.nuget.org/packages/StringTokenFormatter/

Include the using statement so that the extension methods are available:
```C#
using StringTokenFormatter;
```

Usage 1: Passing a single token and value
```C#
string original = "start {middle} end";
string result = original.FormatToken("middle", "centre");
Assert.AreEqual("start centre end", result);
```
Usage 2: Passing a token/value dictionary
```C#
string original = "start {middle} end";
var tokenValues = new Dictionary<string, object> { { "middle", "centre" } };
string result = original.FormatToken(tokenValues);
Assert.AreEqual("start centre end", result);
```
Usage 3: Passing object with properties to be used as token dictionary
```C#
string original = "start {middle} end";
var tokenValues = new { Middle = "centre" };
string result = original.FormatToken(tokenValues);
Assert.AreEqual("start centre end", result);
```
Usage 4: Passing on-demand token value functions (for single or dictionary tokens)
```C#
string original = "start {middle} end";
Func<string, object> func = (token) => { return "centre"; };
string result = original.FormatToken("middle", func);
Assert.AreEqual("start centre end", result);
```

An ```IFormatProvider``` can be passed using the method overloads.

Tokens with formatting and alignment can be specified in the same way as string.Format for example: ```{token,10:D4}``` see this page on MSDN: http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#FormatItem
