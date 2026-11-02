namespace StringTokenFormatter;

public static class TokenValueContainerBuilderSequenceExtensions
{
    extension(TokenValueContainerBuilder builder)
    {
        public TokenValueContainerBuilder AddSequence<T>(string token, IEnumerable<T> values) where T : notnull =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, WrapComplexObjects(builder, values)));

        public TokenValueContainerBuilder AddSequence<T>(string token, IEnumerable<ITokenValueContainer> containers) =>
            builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));

        public TokenValueContainerBuilder AddPrefixedSequence<T>(string prefix, string token, IEnumerable<T> values) where T : notnull =>
            builder.AddPrefixedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, WrapComplexObjects(builder, values)));

        public TokenValueContainerBuilder AddPrefixedSequence<T>(string prefix, string token, IEnumerable<ITokenValueContainer> containers) =>
            builder.AddPrefixedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));
    }

    private static IEnumerable<object> WrapComplexObjects<T>(TokenValueContainerBuilder builder, IEnumerable<T> values) where T : notnull =>
        typeof(T) == typeof(string) || (typeof(T).IsValueType && PropertyCache<T>.Count == 0)
            ? values.Cast<object>()
            : values.Select(v => TokenValueContainerFactory.FromObject(builder.Settings, v));
}