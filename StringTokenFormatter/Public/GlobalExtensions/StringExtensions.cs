namespace StringTokenFormatter;

public static class StringExtensions
{
    extension(string source)
    {
        public string FormatFromSingle<T>(string token, T replacementValue) where T : notnull =>
        FormatFromSingle(source, token, replacementValue, StringTokenFormatterSettings.Global);
        public string FormatFromSingle<T>(string token, T replacementValue, StringTokenFormatterSettings settings) where T : notnull =>
            FormatFromContainer(source, TokenValueContainerFactory.FromSingle(settings, token, replacementValue), settings);

        public string FormatFromPairs<T>(IEnumerable<KeyValuePair<string, T>> values) =>
            FormatFromPairs(source, values, StringTokenFormatterSettings.Global);
        public string FormatFromPairs<T>(IEnumerable<KeyValuePair<string, T>> values, StringTokenFormatterSettings settings) =>
            FormatFromContainer(source, TokenValueContainerFactory.FromPairs(settings, values), settings);

        public string FormatFromTuples<T>(IEnumerable<(string TokenName, T Value)> values) =>
            FormatFromTuples(source, values, StringTokenFormatterSettings.Global);
        public string FormatFromTuples<T>(IEnumerable<(string TokenName, T Value)> values, StringTokenFormatterSettings settings) =>
            FormatFromContainer(source, TokenValueContainerFactory.FromTuples(settings, values), settings);

        public string FormatFromObject<T>(T valuesObject) where T : class =>
            FormatFromObject(source, valuesObject, StringTokenFormatterSettings.Global);
        public string FormatFromObject<T>(T valuesObject, StringTokenFormatterSettings settings) where T : class =>
            FormatFromContainer(source, TokenValueContainerFactory.FromObject(settings, valuesObject), settings);

        public string FormatFromFunc<T>(Func<string, T> func) =>
            FormatFromFunc(source, func, StringTokenFormatterSettings.Global);
        public string FormatFromFunc<T>(Func<string, T> func, StringTokenFormatterSettings settings) =>
            FormatFromContainer(source, TokenValueContainerFactory.FromFunc(settings, func), settings);

        public string FormatFromContainer(ITokenValueContainer container) =>
            FormatFromContainer(source, container, StringTokenFormatterSettings.Global);
        public string FormatFromContainer(ITokenValueContainer container, StringTokenFormatterSettings settings) =>
             InterpolatedStringExpander.Expand(InterpolatedStringParser.Parse(source, settings), container);
    }
}
