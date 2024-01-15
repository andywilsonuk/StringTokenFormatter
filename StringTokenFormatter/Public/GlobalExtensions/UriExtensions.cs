namespace StringTokenFormatter;

public static class UriExtensions
{    
    public static Uri FormatFromSingle<T>(this Uri source, string token, T replacementValue) where T : notnull =>
        FormatFromSingle(source, token, replacementValue, StringTokenFormatterSettings.Global);
    public static Uri FormatFromSingle<T>(this Uri source, string token, T replacementValue, StringTokenFormatterSettings settings) where T : notnull =>
        FormatFromContainer(source, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

    public static Uri FormatFromPairs<T>(this Uri source, IEnumerable<KeyValuePair<string, T>> values) =>
        FormatFromPairs(source, values, StringTokenFormatterSettings.Global);
    public static Uri FormatFromPairs<T>(this Uri source, IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromPairs(settings, values), settings);

    public static Uri FormatFromTuples<T>(this Uri source, IEnumerable<(string TokenName, T Value)> values) =>
        FormatFromTuples(source, values, StringTokenFormatterSettings.Global);
    public static Uri FormatFromTuples<T>(this Uri source, IEnumerable<(string TokenName, T Value)> values, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromTuples(settings, values), settings);

    public static Uri FormatFromObject<T>(this Uri source, T valuesObject) where T : class =>
        FormatFromObject(source, valuesObject, StringTokenFormatterSettings.Global);
    public static Uri FormatFromObject<T>(this Uri source, T valuesObject, StringTokenFormatterSettings settings) where T : class =>
        FormatFromContainer(source, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

    public static Uri FormatFromFunc<T>(this Uri source, Func<string, T> func) =>
        FormatFromFunc(source, func, StringTokenFormatterSettings.Global);
    public static Uri FormatFromFunc<T>(this Uri source, Func<string, T> func, StringTokenFormatterSettings settings) =>
        FormatFromContainer(source, TokenValueContainerFactory.FromFunc(settings, func), settings);

    public static Uri FormatFromContainer(this Uri source, ITokenValueContainer container) =>
        FormatFromContainer(source, container, StringTokenFormatterSettings.Global);
    public static Uri FormatFromContainer(this Uri source, ITokenValueContainer container, StringTokenFormatterSettings settings) =>
        new(InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(source.OriginalString, settings), container), UriKind.RelativeOrAbsolute);
}
