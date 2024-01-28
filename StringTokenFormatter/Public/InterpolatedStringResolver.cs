namespace StringTokenFormatter;

public class InterpolatedStringResolver
{
    private readonly ExpanderValueFormatter formatter;

    public StringTokenFormatterSettings Settings { get; }

    public InterpolatedStringResolver(StringTokenFormatterSettings settings)
    {
        Settings = Guard.NotNull(settings, nameof(settings));
        formatter = new ExpanderValueFormatter(settings.FormatterDefinitions, settings.NameComparer);
    }

    public string FromSingle<T>(string interpolatedString, string token, T value) where T : notnull =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromSingle(Settings, token, value));
    public string FromSingle<T>(InterpolatedString interpolatedString, string token, T value) where T : notnull =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromSingle(Settings, token, value));

    public string FromPairs<T>(string interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    public string FromPairs<T>(string interpolatedString, params KeyValuePair<string, T>[] pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    public string FromPairs<T>(InterpolatedString interpolatedString, IEnumerable<KeyValuePair<string, T>> pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));
    public string FromPairs<T>(InterpolatedString interpolatedString, params KeyValuePair<string, T>[] pairs) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromPairs(Settings, pairs));

    public string FromTuples<T>(string interpolatedString, IEnumerable<(string TokenName, T Value)> tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public string FromTuples<T>(string interpolatedString, params (string TokenName, T Value)[] tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public string FromTuples<T>(InterpolatedString interpolatedString, IEnumerable<(string TokenName, T Value)> tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public string FromTuples<T>(InterpolatedString interpolatedString, params (string TokenName, T Value)[] tuples) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public string FromObject<T>(string interpolatedString, T containerObject) where T : class =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromObject(Settings, containerObject));
    public string FromObject<T>(InterpolatedString interpolatedString, T containerObject) where T : class =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromObject(Settings, containerObject));

    public string FromFunc<T>(string interpolatedString, Func<string, T> func) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromFunc(Settings, func));
    public string FromFunc<T>(InterpolatedString interpolatedString, Func<string, T> func) =>
        FromContainer(interpolatedString, TokenValueContainerFactory.FromFunc(Settings, func));
        
    public string FromContainer(string interpolatedString, ITokenValueContainer tokenValueContainer) =>
       FromContainer(InterpolatedStringParser.Parse(interpolatedString, Settings), tokenValueContainer);
    public string FromContainer(InterpolatedString segments, ITokenValueContainer tokenValueContainer) =>
        InterpolatedStringExpander.Expand(segments, tokenValueContainer, formatter);
}
