namespace StringTokenFormatter;

public static class StringExtensions
{
    public static string FormatFromSingle<T>(this string source, string token, T replacementValue) =>
        FormatFromSingle(source, token, replacementValue, StringTokenFormatterSettings.Global);
    public static string FormatFromSingle<T>(this string source, string token, T replacementValue, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

    public static string FormatFromPairs<T>(this string source, IEnumerable<KeyValuePair<string, T>> values) =>
        FormatFromPairs(source, values, StringTokenFormatterSettings.Global);
    public static string FormatFromPairs<T>(this string source, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromPairs(settings, values), settings);

    public static string FormatFromObject<T>(this string source, T valuesObject) =>
        FormatFromObject(source, valuesObject, StringTokenFormatterSettings.Global);
    public static string FormatFromObject<T>(this string source, T valuesObject, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

    public static string FormatFromFunc<T>(this string source, Func<string, T> func) =>
        FormatFromFunc(source, func, StringTokenFormatterSettings.Global);
    public static string FormatFromFunc<T>(this string source, Func<string, T> func, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromFunc(settings, func), settings);

    public static string FormatFromContainer(this string source, ITokenValueContainer container) =>
        FormatFromContainer(source, container, StringTokenFormatterSettings.Global);
    public static string FormatFromContainer(this string source, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
         InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(source, settings), container);
}
