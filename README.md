#StringTokenFormatter
Provides string extension methods for replacing tokens within strings (using the format '{name}') with their specified lookup value.

Available on NuGet at https://www.nuget.org/packages/StringTokenFormatter/

Include the using statement so that the extension methods are available:
```C#
using StringTokenFormatter;
```

Usage 1:
```C#
string original = "replace {this} value";
string result = original.FormatToken("this", "that");
Assert.AreEqual("replace that value", result);
```
Usage 2:
```C#
string original = "replace {this} value";
var tokenValues = new Dictionary<string, object> { { "this", "that" } };
string result = original.FormatToken(tokenValues);
Assert.AreEqual("replace that value", result);
```
Usage 3:
```C#
string original = "replace {athis} value";
var tokenValues = new { athis = "that" };
string result = original.FormatToken(tokenValues);
Assert.AreEqual("replace that value", result);
```

An ```IFormatProvider``` can be passed using the method overloads.

Tokens with formatting and alignment can be specified in the same way as string.Format for example: ```{token,10:D4}``` see this page on MSDN: http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#FormatItem
