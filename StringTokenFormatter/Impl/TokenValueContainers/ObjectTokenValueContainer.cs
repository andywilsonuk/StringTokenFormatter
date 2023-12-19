#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

public sealed class ObjectTokenValueContainer<T> : ITokenValueContainer where T : class
{
    private static readonly PropertyCache<T> propertyCache = new();
    private readonly ITokenValueContainerSettings settings;
    private IDictionary<string, NonLockingLazy<object>> pairs;

    internal ObjectTokenValueContainer(ITokenValueContainerSettings settings, T source)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        ValidateArgs.AssertNotNull(source, nameof(source));

        pairs = propertyCache.GetPairs().ToDictionary(
            p => p.Property.Name,
            p => new NonLockingLazy<object>(() => p.Getter(source)),
            settings.NameComparer);
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
