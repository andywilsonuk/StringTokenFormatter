# StringTokenFormatter

Usage 1:
string result = "replace {this} value".FormatToken("this", "that");

Usage 2:
var tokenValues = new Dictionary<string, object> { { "this", "that" } };
string result = "replace {this} value".FormatToken(tokenValues);

Usage 3:
var tokenValues = new { this = "that" };
string result = "replace {this} value".FormatToken(tokenValues);

An IFormatProvider can be passed using the method overloads.
Tokens with formatting and alignment can be specified in the same way as string.Format for example: {token,10:D4} see this page on MSDN: http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#FormatItem
