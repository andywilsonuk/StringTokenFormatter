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

    public TokenValueContainerBuilder AddPairs<T>(IEnumerable<KeyValuePair<string, T>> pairs) =>
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

    public TokenValueContainerBuilder AddNestedSingle<T>(string prefix, string token, T value) where T : notnull =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromSingle(Settings, token, value));

    public TokenValueContainerBuilder AddNestedPairs<T>(string prefix, IEnumerable<KeyValuePair<string, T>> pairs) =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromPairs(Settings, pairs));

    public TokenValueContainerBuilder AddNestedTuples<T>(string prefix, IEnumerable<(string, T)> tuples) =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public TokenValueContainerBuilder AddNestedObject<T>(string prefix, T source) where T : class =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromObject(Settings, source));

    public TokenValueContainerBuilder AddNestedFunc<T>(string prefix, Func<string, T> func) =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromFunc(Settings, func));

    public TokenValueContainerBuilder AddNestedContainer(string prefix, ITokenValueContainer tokenValueContainer) =>
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