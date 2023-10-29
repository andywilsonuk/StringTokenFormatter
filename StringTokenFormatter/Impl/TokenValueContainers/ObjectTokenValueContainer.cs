namespace StringTokenFormatter.Impl;

/// <summary>
/// Converts only the properties exposed by {T} (but not any members on derived classes) to a token value container.
/// </summary>
/// <typeparam name="T">A type indicating the exact properties that will be used for formatting.</typeparam>
public class ObjectTokenValueContainer<T> : ITokenValueContainer
{
    private static readonly PropertyCache<T> propertyCache = new();
    private readonly IDictionary<string, NonLockingLazy<object>> dictionary;
    private readonly ITokenValueContainerSettings settings;

    public ObjectTokenValueContainer(T source, ITokenValueContainerSettings settings)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        dictionary = propertyCache.GetPairs().ToDictionary(
            p => p.Property.Name,
            p => new NonLockingLazy<object>(() => p.Getter(source)),
            settings.NameComparer);
    }

    public TryGetResult TryMap(string token) =>
        dictionary.TryGetValue(token, out var value) && settings.TokenResolutionPolicy.Satisfies(value.Value) ? TryGetResult.Success(value.Value) : default;
}
