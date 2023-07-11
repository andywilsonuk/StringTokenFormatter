namespace StringTokenFormatter.Impl;

public static class InterpolatedStringResolver
{
    public static string Expand(string interpolatedString, ITokenValueContainer tokenValueContainer, StringTokenFormatterSettings settings) =>
        InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(interpolatedString, settings), tokenValueContainer);
    public static string Expand(InterpolatedString segments, ITokenValueContainer tokenValueContainer) =>
        InterpolatedStringExpander.Expand(segments, tokenValueContainer);

    public static string FromSingle<T>(string interpolatedString, string token, T value, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromSingle(settings, token, value), settings);
    public static string FromSingle<T>(InterpolatedString interpolatedString, string token, T value, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromSingle(settings, token, value));

    public static string FromPairs<T>(string interpolatedString, IEnumerable<TokenValue<T>> pairs, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromDictionary(settings, pairs), settings);
    public static string FromPairs<T>(InterpolatedString interpolatedString, IEnumerable<TokenValue<T>> pairs, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromDictionary(settings, pairs));
    public static string FromPairs<T>(string interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromDictionary(settings, pairs.Select(p => new TokenValue<T>(p.Key, p.Value))), settings);
    public static string FromPairs<T>(InterpolatedString interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromDictionary(settings, pairs.Select(p => new TokenValue<T>(p.Key, p.Value))));

    public static string FromObject<T>(string interpolatedString, T containerObject, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromObject(settings, containerObject), settings);
    public static string FromObject<T>(InterpolatedString interpolatedString, T containerObject, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromObject(settings, containerObject));

    public static string FromFunc<T>(string interpolatedString, Func<string, T> func, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromFunc(settings, func), settings);
    public static string FromFunc<T>(InterpolatedString interpolatedString, Func<string, T> func, StringTokenFormatterSettings settings) =>
        Expand(interpolatedString, TokenValueContainerFactory.FromFunc(settings, func));
}
