using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class UriExtensions
{
    private static Uri Expand(Uri interpolatedString, ITokenValueContainer tokenValueContainer, IInterpolatedStringSettings settings) =>
        new(InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(interpolatedString.OriginalString, settings), tokenValueContainer), UriKind.RelativeOrAbsolute);

    public static Uri FormatToken<T>(this Uri input, T valuesObject) =>
        FormatToken(input, valuesObject, StringTokenFormatterSettings.Global);
    public static Uri FormatToken<T>(this Uri input, T valuesObject, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue) =>
        FormatToken(input, token, replacementValue, StringTokenFormatterSettings.Global);
    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

    public static Uri FormatToken<T>(this Uri input, Func<string, T> func) =>
        FormatToken(input, func, StringTokenFormatterSettings.Global);
    public static Uri FormatToken<T>(this Uri input, Func<string, T> func, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromFunc(settings, func), settings);

    public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values) =>
        FormatToken(input, values, StringTokenFormatterSettings.Global);
    public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        Expand(input, TokenValueContainerFactory.FromDictionary<T>(settings, values.Select(TokenValue<T>.FromPair)), settings);

    public static Uri FormatContainer(this Uri input, ITokenValueContainer container) =>
        FormatToken(input, container, StringTokenFormatterSettings.Global);
    public static Uri FormatContainer(this Uri input, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
        Expand(input, container, settings);
}
