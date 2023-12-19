#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

public sealed class ObjectTokenValueContainer<T> : ITokenValueContainer where T : class
{
    private static readonly PropertyCache<T> propertyCache = new();
    private readonly ITokenValueContainerSettings settings;
    private IDictionary<string, NonLockingLazy> pairs;

    internal ObjectTokenValueContainer(ITokenValueContainerSettings settings, T source)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        this.pairs = CreateDictionary(ValidateArgs.AssertNotNull(source, nameof(source)));
    }

    private Dictionary<string, NonLockingLazy> CreateDictionary(T source)
    {
        var d = new Dictionary<string, NonLockingLazy>(settings.NameComparer);
        foreach (var (propertyName, getValue) in propertyCache.GetPairs())
        {
            d.Add(propertyName, new NonLockingLazy(() => getValue(source)));
        }
        if (d.Count == 0) { throw new TokenContainerException("The container is empty"); }
        return d;
    }

    public TryGetResult TryMap(string token) =>
        pairs.TryGetValue(token, out var lazy) && settings.TokenResolutionPolicy.Satisfies(lazy.Value) ? TryGetResult.Success(lazy.Value) : default;

#if NET8_0_OR_GREATER
    /// <summary>
    /// Converts the inner dictionary to a FrozenDictionary for faster read performance if reused.
    /// </summary>
    public void Frozen()
    {
        pairs = pairs.ToFrozenDictionary();
    }
#endif
}
