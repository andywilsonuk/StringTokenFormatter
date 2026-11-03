namespace StringTokenFormatter.Impl;

public sealed class DictionaryTokenValueContainer<T> : ITokenValueContainer
{
    private readonly ITokenValueContainerSettings settings;
    private readonly bool resolveNestedValues;
    private readonly string hierarchicalDelimiter;
    private IDictionary<string, T> pairs;

    internal DictionaryTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<(string TokenName, T Value)> source)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        resolveNestedValues = NestedTokenValueResolver.IsEnabled(this.settings);
        hierarchicalDelimiter = NestedTokenValueResolver.GetDelimiter(this.settings);
        this.pairs = CreateDictionary(Guard.NotNull(source, nameof(source)));
    }

    internal DictionaryTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<KeyValuePair<string, T>> source)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        resolveNestedValues = NestedTokenValueResolver.IsEnabled(this.settings);
        hierarchicalDelimiter = NestedTokenValueResolver.GetDelimiter(this.settings);
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

    public TryGetResult TryMap(string token)
    {
        if (pairs.TryGetValue(token, out T? value)) { return TryGetResult.Success(value); }
        return resolveNestedValues ? TryMapNested(token) : default;
    }

    private TryGetResult TryMapNested(string token)
    {
        int delimiterIndex = token.IndexOf(hierarchicalDelimiter, StringComparison.Ordinal);
        if (delimiterIndex < 0 || !pairs.TryGetValue(token[..delimiterIndex], out T? rootValue)) { return default; }
        return NestedTokenValueResolver.TryMapSegments(token, delimiterIndex + hierarchicalDelimiter.Length, rootValue, hierarchicalDelimiter, settings.NameComparer);
    }

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
