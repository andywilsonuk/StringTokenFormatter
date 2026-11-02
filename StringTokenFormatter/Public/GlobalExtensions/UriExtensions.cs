namespace StringTokenFormatter;

public static class UriExtensions
{
    extension(Uri source)
    {
        public Uri FormatFromSingle<T>(string token, T replacementValue) where T : notnull =>
        FormatFromSingle(source, token, replacementValue, StringTokenFormatterSettings.Global);
        public Uri FormatFromSingle<T>(string token, T replacementValue, StringTokenFormatterSettings settings) where T : notnull =>
            FormatFromContainer(source, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

        public Uri FormatFromPairs<T>(IEnumerable<KeyValuePair<string, T>> values) =>
            FormatFromPairs(source, values, StringTokenFormatterSettings.Global);
        public Uri FormatFromPairs<T>(IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
            FormatFromContainer(source, TokenValueContainerFactory.FromPairs(settings, values), settings);

        public Uri FormatFromTuples<T>(IEnumerable<(string TokenName, T Value)> values) =>
            FormatFromTuples(source, values, StringTokenFormatterSettings.Global);
        public Uri FormatFromTuples<T>(IEnumerable<(string TokenName, T Value)> values, StringTokenFormatterSettings settings) =>
            FormatFromContainer(source, TokenValueContainerFactory.FromTuples(settings, values), settings);

        public Uri FormatFromObject<T>(T valuesObject) where T : class =>
            FormatFromObject(source, valuesObject, StringTokenFormatterSettings.Global);
        public Uri FormatFromObject<T>(T valuesObject, StringTokenFormatterSettings settings) where T : class =>
            FormatFromContainer(source, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

        public Uri FormatFromFunc<T>(Func<string, T> func) =>
            FormatFromFunc(source, func, StringTokenFormatterSettings.Global);
        public Uri FormatFromFunc<T>(Func<string, T> func, StringTokenFormatterSettings settings) =>
            FormatFromContainer(source, TokenValueContainerFactory.FromFunc(settings, func), settings);

        public Uri FormatFromContainer(ITokenValueContainer container) =>
            FormatFromContainer(source, container, StringTokenFormatterSettings.Global);
        public Uri FormatFromContainer(ITokenValueContainer container, StringTokenFormatterSettings settings) =>
            new(InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(source.OriginalString, settings), container), UriKind.RelativeOrAbsolute);
    }
}
