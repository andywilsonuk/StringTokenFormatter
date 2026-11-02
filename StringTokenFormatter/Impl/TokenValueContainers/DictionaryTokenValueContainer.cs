namespace StringTokenFormatter.Impl;

public sealed class DictionaryTokenValueContainer<T> : ITokenValueContainer
{
    private readonly ITokenValueContainerSettings settings;
    private IDictionary<string, T> pairs;

    internal DictionaryTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<(string TokenName, T Value)> source)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        this.pairs = CreateDictionary(Guard.NotNull(source, nameof(source)));
    }

    internal DictionaryTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<KeyValuePair<string, T>> source)
    {
        this.settings = Guard.NotNull(settings, nameof(settings));
        this.pairs = CreateDictionary(Guard.NotNull(source, nameof(source)).Select(x => (x.Key, x.Value)));
    }

    private Dictionary<string, T> CreateDictionary(IEnumerable<(string TokenName, T Value)> source)
    {
        Dictionary<string, T> d = new(settings.NameComparer);
        foreach ((string? tokenName, T? value) in source)
        {
            if (d.ContainsKey(tokenName)) { throw new TokenContainerException($"The container already has a token with name '{tokenName}'"); }
            d.Add(tokenName, value);
        }
        if (d.ContainsKey(string.Empty)) { throw new TokenContainerException("Empty string cannot be used as token name"); }
        if (d.Count == 0) { throw new TokenContainerException("The container is empty"); }
        return d;
    }

    public TryGetResult TryMap(string token) => pairs.TryGetValue(token, out T? value) ? TryGetResult.Success(value) : default;

    /// <summary>
    /// Converts the inner dictionary to a FrozenDictionary for faster read performance if reused on .NET 8 or later.
    /// This method has no effect on earlier target frameworks.
    /// </summary>
    public DictionaryTokenValueContainer<T> Frozen()
    {
        pairs = pairs.ToFrozenDictionaryIfSupported();
        return this;
    }
}
