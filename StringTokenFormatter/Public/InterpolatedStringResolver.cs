namespace StringTokenFormatter;

public class InterpolatedStringResolver
{
    public static InterpolatedStringResolver Default { get; } = new(StringTokenFormatterSettings.Default);

    private readonly ExpanderValueFormatter formatter;

    public StringTokenFormatterSettings Settings { get; }

    public InterpolatedStringResolver(StringTokenFormatterSettings settings)
    {
        Settings = Guard.NotNull(settings, nameof(settings)).Validate();
        formatter = new ExpanderValueFormatter(settings.FormatterDefinitions, settings.NameComparer);
    }

    /// <summary>
    /// Resolves the interpolated string where a single token name and value is required.
    /// </summary>
    public string FromSingle<T>(string interpolatedString, string token, T value) where T : notnull =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromSingle(Settings, token, value));
    /// <summary>
    /// Resolves the interpolated string where a single token name and value is required.
    /// </summary>
    public string FromSingle<T>(InterpolatedString interpolatedString, string token, T value) where T : notnull =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromSingle(Settings, token, value));

    /// <summary>
    /// Resolves the interpolated string using key/value pairs for token matching.
    /// </summary>
    public string FromPairs<T>(string interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    /// <summary>
    /// Resolves the interpolated string using key/value pairs for token matching.
    /// </summary>
    public string FromPairs<T>(string interpolatedString, params KeyValuePair<string, T>[] pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    /// <summary>
    /// Resolves the interpolated string using key/value pairs for token matching.
    /// </summary>
    public string FromPairs<T>(InterpolatedString interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    /// <summary>
    /// Resolves the interpolated string using key/value pairs for token matching.
    /// </summary>
    public string FromPairs<T>(InterpolatedString interpolatedString, params KeyValuePair<string, T>[] pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));

    /// <summary>
    /// Resolves the interpolated string using key/value tuples for token matching.
    /// </summary>
    public string FromTuples<T>(string interpolatedString, IEnumerable<(string TokenName, T Value)> tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));
    /// <summary>
    /// Resolves the interpolated string using key/value tuples for token matching.
    /// </summary>
    public string FromTuples<T>(string interpolatedString, params (string TokenName, T Value)[] tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));
    /// <summary>
    /// Resolves the interpolated string using key/value tuples for token matching.
    /// </summary>
    public string FromTuples<T>(InterpolatedString interpolatedString, IEnumerable<(string TokenName, T Value)> tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));
    /// <summary>
    /// Resolves the interpolated string using key/value tuples for token matching.
    /// </summary>
    public string FromTuples<T>(InterpolatedString interpolatedString, params (string TokenName, T Value)[] tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));

    /// <summary>
    /// Resolves the interpolated string using token matching provided by properties exposed by {T} (but not any members on derived classes).
    /// </summary>
    public string FromObject<T>(string interpolatedString, T containerObject) where T : class =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromObject(Settings, containerObject));
    /// <summary>
    /// Resolves the interpolated string using token matching provided by properties exposed by {T} (but not any members on derived classes).
    /// </summary>
    public string FromObject<T>(InterpolatedString interpolatedString, T containerObject) where T : class =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromObject(Settings, containerObject));

    /// <summary>
    /// Resolves the interpolated string using a func delegate for token matching.
    /// </summary>
    public string FromFunc<T>(string interpolatedString, Func<string, T> func) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromFunc(Settings, func));
    /// <summary>
    /// Resolves the interpolated string using a func delegate for token matching.
    /// </summary>
    public string FromFunc<T>(InterpolatedString interpolatedString, Func<string, T> func) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromFunc(Settings, func));

    /// <summary>
    /// Resolves the interpolated string by searching child containers in order added for the first valid token value. 
    /// </summary>
    public string FromContainer(string interpolatedString, ITokenValueContainer tokenValueContainer) =>
       FromContainer(Interpolate(interpolatedString), tokenValueContainer);
    /// <summary>
    /// Resolves the interpolated string by searching child containers in order added for the first valid token value. 
    /// </summary>
    public string FromContainer(InterpolatedString segments, ITokenValueContainer tokenValueContainer) =>
        InterpolatedStringExpander.Expand(segments, tokenValueContainer, formatter);

    /// <summary>
    /// Parses the raw string into an `InterpolatedString`.
    /// </summary>
    public InterpolatedString Interpolate(string interpolatedString) => InterpolatedStringParser.Parse(interpolatedString, Settings);

    /// <summary>
    /// Get a new instance of the container builder using the Resolver settings.
    /// </summary>
    public TokenValueContainerBuilder Builder() => new(Settings);
}
