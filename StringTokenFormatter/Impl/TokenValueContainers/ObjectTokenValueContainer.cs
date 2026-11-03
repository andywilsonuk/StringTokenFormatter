namespace StringTokenFormatter.Impl;

public sealed class ObjectTokenValueContainer<T> : ITokenValueContainer where T : notnull
{
    private readonly ITokenValueContainerSettings settings;
    private readonly T source;
    private readonly bool resolveNestedValues;
    private readonly string hierarchicalDelimiter;
    private IDictionary<string, NonLockingLazy> pairs;

    internal ObjectTokenValueContainer(ITokenValueContainerSettings settings, T source)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        this.source = Guard.NotNull(source, nameof(source));
        resolveNestedValues = NestedTokenValueResolver.IsEnabled(this.settings);
        hierarchicalDelimiter = NestedTokenValueResolver.GetDelimiter(this.settings);
        this.pairs = CreateDictionary(this.source);
    }

    private Dictionary<string, NonLockingLazy> CreateDictionary(T source)
    {
        Dictionary<string, NonLockingLazy> d = new(settings.NameComparer);
        foreach ((string? propertyName, Func<T, object>? getValue) in PropertyCache<T>.Properties)
        {
            d.Add(propertyName, new NonLockingLazy(() => getValue(source)));
        }
        if (d.Count == 0) { throw new TokenContainerException("The container is empty"); }
        return d;
    }

    public TryGetResult TryMap(string token)
    {
        if (pairs.TryGetValue(token, out NonLockingLazy? lazy)) { return TryGetResult.Success(lazy.Value); }
        return resolveNestedValues ? TryMapNested(token) : default;
    }

    private TryGetResult TryMapNested(string token)
    {
        int delimiterIndex = token.IndexOf(hierarchicalDelimiter, StringComparison.Ordinal);
        if (delimiterIndex < 0) { return default; }

        // The root segment is resolved from the pre-built property pairs, falling back to
        // direct child resolution for sources such as dictionaries whose entries are not properties.
        object? rootValue;
        string rootSegment = token[..delimiterIndex];
        if (pairs.TryGetValue(rootSegment, out NonLockingLazy? rootLazy))
        {
            rootValue = rootLazy.Value;
        }
        else if (!NestedPropertyCache.TryGetChildValue(source, rootSegment, settings.NameComparer, out rootValue))
        {
            return default;
        }
        return NestedTokenValueResolver.TryMapSegments(token, delimiterIndex + hierarchicalDelimiter.Length, rootValue, hierarchicalDelimiter, settings.NameComparer);
    }

    /// <summary>
    /// Converts the inner dictionary to a FrozenDictionary for faster read performance if reused on .NET 8 or later.
    /// This method has no effect on earlier target frameworks.
    /// </summary>
    public void Frozen()
    {
        pairs = pairs.ToFrozenDictionaryIfSupported();
    }
}
