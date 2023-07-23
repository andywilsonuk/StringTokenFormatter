using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class UriExtensions
{
    public static Uri FormatToken<T>(this Uri input, T valuesObject) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromObject(s, valuesObject, StringTokenFormatterSettings.Global));
    public static Uri FormatToken<T>(this Uri input, T valuesObject, StringTokenFormatterSettings settings) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromObject(s, valuesObject, settings));

    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromSingle(s, token, replacementValue, StringTokenFormatterSettings.Global));
    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromSingle(s, token, replacementValue, settings));

    public static Uri FormatToken<T>(this Uri input, Func<string, T> func) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromFunc(s, func, StringTokenFormatterSettings.Global));
    public static Uri FormatToken<T>(this Uri input, Func<string, T> func, StringTokenFormatterSettings settings) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromFunc(s, func, settings));

    public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromPairs(s, values, StringTokenFormatterSettings.Global));
    public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        UriWrapper(input, s => InterpolatedStringResolver.FromPairs(s, values, settings));

    public static Uri FormatContainer(this Uri input, ITokenValueContainer container) =>
        UriWrapper(input, s => InterpolatedStringResolver.Expand(s, container, StringTokenFormatterSettings.Global));
    public static Uri FormatContainer(this Uri input, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
        UriWrapper(input, s => InterpolatedStringResolver.Expand(s, container, settings));

    private static Uri UriWrapper(Uri input, Func<string, string> expander) => new(expander(input.OriginalString), UriKind.RelativeOrAbsolute);

}
