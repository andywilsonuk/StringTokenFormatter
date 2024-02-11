namespace StringTokenFormatter;

public static class TokenValueContainerBuilderSequenceExtensions
{
    public static TokenValueContainerBuilder AddSequence<T>(this TokenValueContainerBuilder builder, string token, IEnumerable<T> values) where T : notnull =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, WrapComplexObjects(builder, values)));

    public static TokenValueContainerBuilder AddSequence<T>(this TokenValueContainerBuilder builder, string token, IEnumerable<ITokenValueContainer> containers) =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));

    public static TokenValueContainerBuilder AddNestedSequence<T>(this TokenValueContainerBuilder builder, string prefix, string token, IEnumerable<T> values) where T : notnull =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, WrapComplexObjects(builder, values)));

    public static TokenValueContainerBuilder AddNestedSequence<T>(this TokenValueContainerBuilder builder, string prefix, string token, IEnumerable<ITokenValueContainer> containers) =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));
    
    private static IEnumerable<object> WrapComplexObjects<T>(TokenValueContainerBuilder builder, IEnumerable<T> values) where T : notnull =>
        typeof(T) == typeof(string) || (typeof(T).IsValueType && PropertyCache<T>.Count == 0)
            ? values.Cast<object>()
            : values.Select(v => TokenValueContainerFactory.FromObject(builder.Settings, v));
}