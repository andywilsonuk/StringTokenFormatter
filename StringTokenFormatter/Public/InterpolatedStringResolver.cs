namespace StringTokenFormatter;

public class InterpolatedStringResolver
{
    public StringTokenFormatterSettings Settings { get; }

    public InterpolatedStringResolver(StringTokenFormatterSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public string FromSingle<T>(string interpolatedString, string token, T value) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromSingle(Settings, token, value));
    public string FromSingle<T>(InterpolatedString interpolatedString, string token, T value) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromSingle(Settings, token, value));

    public string FromPairs<T>(string interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    public string FromPairs<T>(InterpolatedString interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));

    public string FromObject<T>(string interpolatedString, T containerObject) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromObject(Settings, containerObject));
    public string FromObject<T>(InterpolatedString interpolatedString, T containerObject) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromObject(Settings, containerObject));

    public string FromFunc<T>(string interpolatedString, Func<string, T> func) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromFunc(Settings, func));
    public string FromFunc<T>(InterpolatedString interpolatedString, Func<string, T> func) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromFunc(Settings, func));
        
    public string FromContainer(string interpolatedString, ITokenValueContainer tokenValueContainer) =>
        InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(interpolatedString, Settings), tokenValueContainer);
    public string FromContainer(InterpolatedString segments, ITokenValueContainer tokenValueContainer) =>
        InterpolatedStringExpander.Expand(segments, tokenValueContainer);
}
