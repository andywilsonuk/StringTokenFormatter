namespace StringTokenFormatter;

public static class TokenValueContainerBuilderSequenceExtensions
{
    public static void AddSequence(this TokenValueContainerBuilder builder, string token, IEnumerable<object> values) => 
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddSequence(this TokenValueContainerBuilder builder, string token, params object[] values) =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddSequence(this TokenValueContainerBuilder builder, string token, IEnumerable<string> values) => 
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddSequence(this TokenValueContainerBuilder builder, string token, params string[] values) =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddSequence<T>(this TokenValueContainerBuilder builder, string token, IEnumerable<T> values) where T : class =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, values.Select(v => TokenValueContainerFactory.FromObject(builder.Settings, v))));

    public static void AddSequence<T>(this TokenValueContainerBuilder builder, string token, params T[] values) where T : class =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, values.Select(v => TokenValueContainerFactory.FromObject(builder.Settings, v))));

    public static void AddSequence<T>(this TokenValueContainerBuilder builder, string token, IEnumerable<ITokenValueContainer> containers) where T : notnull =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));

    public static void AddSequence<T>(this TokenValueContainerBuilder builder, string token, params ITokenValueContainer[] containers) where T : notnull =>
        builder.AddContainer(TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));

    public static void AddNestedSequence(this TokenValueContainerBuilder builder, string prefix, string token, IEnumerable<object> values) => 
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddNestedSequence(this TokenValueContainerBuilder builder, string prefix, string token, params object[] values) =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddNestedSequence(this TokenValueContainerBuilder builder, string prefix, string token, IEnumerable<string> values) => 
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddNestedSequence(this TokenValueContainerBuilder builder, string prefix, string token, params string[] values) =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, values));

    public static void AddNestedSequence<T>(this TokenValueContainerBuilder builder, string prefix, string token, IEnumerable<T> values) where T : class =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, values.Select(v => TokenValueContainerFactory.FromObject(builder.Settings, v))));

    public static void AddNestedSequence<T>(this TokenValueContainerBuilder builder, string prefix, string token, params T[] values) where T : class =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, values.Select(v => TokenValueContainerFactory.FromObject(builder.Settings, v))));

    public static void AddNestedSequence<T>(this TokenValueContainerBuilder builder, string prefix, string token, IEnumerable<ITokenValueContainer> containers) where T : notnull =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));

    public static void AddNestedSequence<T>(this TokenValueContainerBuilder builder, string prefix, string token, params ITokenValueContainer[] containers) where T : notnull =>
        builder.AddNestedContainer(prefix, TokenValueContainerFactory.FromSequence(builder.Settings, token, containers));
}