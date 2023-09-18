using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class StringExtensions
{
    private static string Expand(string interpolatedString, ITokenValueContainer tokenValueContainer, IInterpolatedStringSettings settings) =>
        InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(interpolatedString, settings), tokenValueContainer);

    public static string FormatToken<T>(this string input, T valuesObject) =>
        FormatToken(input, valuesObject, StringTokenFormatterSettings.Global);
    public static string FormatToken<T>(this string input, T valuesObject, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

    public static string FormatToken<T>(this string input, string token, T replacementValue) =>
        FormatToken(input, token, replacementValue, StringTokenFormatterSettings.Global);
    public static string FormatToken<T>(this string input, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

    public static string FormatToken<T>(this string input, Func<string, T> func) =>
        FormatToken(input, func, StringTokenFormatterSettings.Global);
    public static string FormatToken<T>(this string input, Func<string, T> func, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromFunc(settings, func), settings);

    public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values) =>
        FormatToken(input, values, StringTokenFormatterSettings.Global);
    public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromDictionary<T>(settings, values.Select(TokenValue<T>.FromPair)), settings);

    public static string FormatContainer(this string input, ITokenValueContainer container) =>
        FormatToken(input, container, StringTokenFormatterSettings.Global);
    public static string FormatContainer(this string input, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
        Expand(input, container, settings);
}
