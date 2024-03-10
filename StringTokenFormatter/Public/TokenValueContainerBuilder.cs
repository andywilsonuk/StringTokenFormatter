namespace StringTokenFormatter;

public class TokenValueContainerBuilder
{
    private readonly List<ITokenValueContainer> innerList = new();

    public StringTokenFormatterSettings Settings { get; }
    public IReadOnlyCollection<ITokenValueContainer> Containers => innerList;

    public TokenValueContainerBuilder(StringTokenFormatterSettings settings)
    {
        Settings = Guard.NotNull(settings, nameof(settings)).Validate();
    }

    public TokenValueContainerBuilder AddSingle<T>(string token, T value) where T : notnull =>
        FluentAdd(TokenValueContainerFactory.FromSingle(Settings, token, value));

    public TokenValueContainerBuilder AddKeyValues<T>(IEnumerable<KeyValuePair<string, T>> pairs) =>
        FluentAdd(TokenValueContainerFactory.FromPairs(Settings, pairs));

    public TokenValueContainerBuilder AddTuples<T>(IEnumerable<(string, T)> pairs) =>
        FluentAdd(TokenValueContainerFactory.FromTuples(Settings, pairs));

    public TokenValueContainerBuilder AddObject<T>(T containerObject) where T : class =>
        FluentAdd(TokenValueContainerFactory.FromObject(Settings, containerObject));

    public TokenValueContainerBuilder AddFunc<T>(Func<string, T> func) =>
        FluentAdd(TokenValueContainerFactory.FromFunc(Settings, func));

    public TokenValueContainerBuilder AddContainer(ITokenValueContainer tokenValueContainer) =>
        FluentAdd(tokenValueContainer);

    public TokenValueContainerBuilder AddContainers(params ITokenValueContainer[] containers) =>
        FluentAdd(containers);

    public TokenValueContainerBuilder AddContainers(IEnumerable<ITokenValueContainer> containers) =>
        FluentAdd(containers);

    public TokenValueContainerBuilder AddPrefixedSingle<T>(string prefix, string token, T value) where T : notnull =>
        AddPrefixedContainer(prefix, TokenValueContainerFactory.FromSingle(Settings, token, value));

    public TokenValueContainerBuilder AddPrefixedKeyValues<T>(string prefix, IEnumerable<KeyValuePair<string, T>> pairs) =>
        AddPrefixedContainer(prefix, TokenValueContainerFactory.FromPairs(Settings, pairs));

    public TokenValueContainerBuilder AddPrefixedTuples<T>(string prefix, IEnumerable<(string, T)> tuples) =>
        AddPrefixedContainer(prefix, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public TokenValueContainerBuilder AddPrefixedObject<T>(string prefix, T source) where T : class =>
        AddPrefixedContainer(prefix, TokenValueContainerFactory.FromObject(Settings, source));

    public TokenValueContainerBuilder AddPrefixedFunc<T>(string prefix, Func<string, T> func) =>
        AddPrefixedContainer(prefix, TokenValueContainerFactory.FromFunc(Settings, func));

    public TokenValueContainerBuilder AddPrefixedContainer(string prefix, ITokenValueContainer tokenValueContainer) =>
        FluentAdd(TokenValueContainerFactory.FromHierarchical(Settings, prefix, tokenValueContainer));

    public ITokenValueContainer CombinedResult() => TokenValueContainerFactory.FromCombination(Settings, innerList);

    private TokenValueContainerBuilder FluentAdd(ITokenValueContainer container)
    {
        innerList.Add(container);
        return this;
    }

    private TokenValueContainerBuilder FluentAdd(IEnumerable<ITokenValueContainer> containers)
    {
        innerList.AddRange(containers);
        return this;
    }
}