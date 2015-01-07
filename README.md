#StringTokenFormatter
Available on NuGet at https://www.nuget.org/packages/StringTokenFormatter/

Usage 1:
```C#
string result = "replace {this} value".FormatToken("this", "that");
Assert.AreEqual("replace that value", result);
```
Usage 2:
```C#
var tokenValues = new Dictionary<string, object> { { "this", "that" } };
string result = "replace {this} value".FormatToken(tokenValues);
Assert.AreEqual("replace that value", result);
```
Usage 3:
```C#
var tokenValues = new { athis = "that" };
string result = "replace {athis} value".FormatToken(tokenValues);
Assert.AreEqual("replace that value", result);
```

An ```IFormatProvider``` can be passed using the method overloads.

Tokens with formatting and alignment can be specified in the same way as string.Format for example: ```{token,10:D4}``` see this page on MSDN: http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#FormatItem
