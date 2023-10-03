using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class UriExtensions
{
    public static Uri FormatToken<T>(this Uri input, T valuesObject) =>
        FormatToken(input, valuesObject, StringTokenFormatterSettings.Global);
    public static Uri FormatToken<T>(this Uri input, T valuesObject, StringTokenFormatterSettings settings) =>
        FormatContainer(input, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue) =>
        FormatToken(input, token, replacementValue, StringTokenFormatterSettings.Global);
    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        FormatContainer(input, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

    public static Uri FormatToken<T>(this Uri input, Func<string, T> func) =>
        FormatToken(input, func, StringTokenFormatterSettings.Global);
    public static Uri FormatToken<T>(this Uri input, Func<string, T> func, StringTokenFormatterSettings settings) =>
        FormatContainer(input, TokenValueContainerFactory.FromFunc(settings, func), settings);

    public static Uri FormatPairs<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values) =>
        FormatPairs(input, values, StringTokenFormatterSettings.Global);
    public static Uri FormatPairs<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        FormatContainer(input, TokenValueContainerFactory.FromPairs(settings, values), settings);

    public static Uri FormatContainer(this Uri input, ITokenValueContainer container) =>
        FormatContainer(input, container, StringTokenFormatterSettings.Global);
    public static Uri FormatContainer(this Uri input, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
        new(InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(input.OriginalString, settings), container), UriKind.RelativeOrAbsolute);
}
