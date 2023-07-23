using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class StringExtensions
{
    public static string FormatToken<T>(this string input, T valuesObject) =>
        InterpolatedStringResolver.FromObject(input, valuesObject, StringTokenFormatterSettings.Global);
    public static string FormatToken<T>(this string input, T valuesObject, StringTokenFormatterSettings settings) =>
        InterpolatedStringResolver.FromObject(input, valuesObject, settings);

    public static string FormatToken<T>(this string input, string token, T replacementValue) =>
        InterpolatedStringResolver.FromSingle(input, token, replacementValue, StringTokenFormatterSettings.Global);
    public static string FormatToken<T>(this string input, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        InterpolatedStringResolver.FromSingle(input, token, replacementValue, settings);

    public static string FormatToken<T>(this string input, Func<string, T> func) =>
        InterpolatedStringResolver.FromFunc(input, func, StringTokenFormatterSettings.Global);
    public static string FormatToken<T>(this string input, Func<string, T> func, StringTokenFormatterSettings settings) =>
        InterpolatedStringResolver.FromFunc(input, func, settings);

    public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values) =>
        InterpolatedStringResolver.FromPairs(input, values, StringTokenFormatterSettings.Global);
    public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        InterpolatedStringResolver.FromPairs(input, values, settings);

    public static string FormatContainer(this string input, ITokenValueContainer container) =>
        InterpolatedStringResolver.Expand(input, container, StringTokenFormatterSettings.Global);
    public static string FormatContainer(this string input, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
        InterpolatedStringResolver.Expand(input, container, settings);
}
