namespace StringTokenFormatter;

public class TokenValueContainerBuilder
{
    private readonly List<ITokenValueContainer> innerList = new();

    public StringTokenFormatterSettings Settings { get; }
    public IReadOnlyCollection<ITokenValueContainer> Containers => innerList;

    public TokenValueContainerBuilder(StringTokenFormatterSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void AddSingle<T>(string token, T value) where T : notnull =>
        innerList.Add(TokenValueContainerFactory.FromSingle(Settings, token, value));

    public void AddPairs<T>(IEnumerable<KeyValuePair<string, T>> pairs) =>
        innerList.Add(TokenValueContainerFactory.FromPairs(Settings, pairs));

    public void AddTuples<T>(IEnumerable<(string, T)> pairs) =>
        innerList.Add(TokenValueContainerFactory.FromTuples(Settings, pairs));

    public void AddObject<T>(T containerObject) where T : class =>
        innerList.Add(TokenValueContainerFactory.FromObject(Settings, containerObject));

    public void AddFunc<T>(Func<string, T> func) =>
        innerList.Add(TokenValueContainerFactory.FromFunc(Settings, func));

    public void AddContainer(ITokenValueContainer tokenValueContainer) =>
        innerList.Add(tokenValueContainer);

    public void AddContainers(params ITokenValueContainer[] containers) =>
        innerList.AddRange(containers);

    public void AddContainers(IEnumerable<ITokenValueContainer> containers) =>
        innerList.AddRange(containers);

    public void AddNestedSingle<T>(string prefix, string token, T value) where T : notnull =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromSingle(Settings, token, value));

    public void AddNestedPairs<T>(string prefix, IEnumerable<KeyValuePair<string, T>> pairs) =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromPairs(Settings, pairs));

    public void AddNestedTuples<T>(string prefix, IEnumerable<(string, T)> tuples) =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromTuples(Settings, tuples));

    public void AddNestedObject<T>(string prefix, T source) where T : class =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromObject(Settings, source));

    public void AddNestedFunc<T>(string prefix, Func<string, T> func) =>
        AddNestedContainer(prefix, TokenValueContainerFactory.FromFunc(Settings, func));

    public void AddNestedContainer(string prefix, ITokenValueContainer tokenValueContainer) =>
        innerList.Add(TokenValueContainerFactory.FromHierarchical(Settings, prefix, tokenValueContainer));

    public ITokenValueContainer CombinedResult() => TokenValueContainerFactory.FromCombination(Settings, innerList);
}