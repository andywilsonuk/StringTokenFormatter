namespace StringTokenFormatter.Impl;

public sealed class ObjectTokenValueContainer<T> : ITokenValueContainer where T : notnull
{
    private readonly ITokenValueContainerSettings settings;
    private IDictionary<string, NonLockingLazy> pairs;

    internal ObjectTokenValueContainer(ITokenValueContainerSettings settings, T source)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        this.pairs = CreateDictionary(Guard.NotNull(source, nameof(source)));
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

    public TryGetResult TryMap(string token) => pairs.TryGetValue(token, out NonLockingLazy? lazy) ? TryGetResult.Success(lazy.Value) : default;

    /// <summary>
    /// Converts the inner dictionary to a FrozenDictionary for faster read performance if reused on .NET 8 or later.
    /// This method has no effect on earlier target frameworks.
    /// </summary>
    public void Frozen()
    {
        pairs = pairs.ToFrozenDictionaryIfSupported();
    }
}
