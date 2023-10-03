using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class StringExtensions
{
    public static string FormatFromObject<T>(this string input, T valuesObject) =>
        FormatFromObject(input, valuesObject, StringTokenFormatterSettings.Global);
    public static string FormatFromObject<T>(this string input, T valuesObject, StringTokenFormatterSettings settings) =>
        FormatFromContainer(input, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

    public static string FormatFromSingle<T>(this string input, string token, T replacementValue) =>
        FormatFromSingle(input, token, replacementValue, StringTokenFormatterSettings.Global);
    public static string FormatFromSingle<T>(this string input, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        FormatFromContainer(input, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

    public static string FormatFromFunc<T>(this string input, Func<string, T> func) =>
        FormatFromFunc(input, func, StringTokenFormatterSettings.Global);
    public static string FormatFromFunc<T>(this string input, Func<string, T> func, StringTokenFormatterSettings settings) =>
        FormatFromContainer(input, TokenValueContainerFactory.FromFunc(settings, func), settings);

    public static string FormatFromPairs<T>(this string input, IEnumerable<KeyValuePair<string, T>> values) =>
        FormatFromPairs(input, values, StringTokenFormatterSettings.Global);
    public static string FormatFromPairs<T>(this string input, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        FormatFromContainer(input, TokenValueContainerFactory.FromPairs(settings, values), settings);

    public static string FormatFromContainer(this string input, ITokenValueContainer container) =>
        InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(input, StringTokenFormatterSettings.Global), container);
    public static string FormatFromContainer(this string input, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
         InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(input, settings), container);
}
