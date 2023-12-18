#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

/// <summary>
/// This Value Container uses key/value pairs for token matching.
/// </summary>
public class DictionaryTokenValueContainer<T> : ITokenValueContainer
{
    private IDictionary<string, T> pairs;
    private readonly ITokenValueContainerSettings settings;

    public DictionaryTokenValueContainer(IEnumerable<(string, T)> source, ITokenValueContainerSettings settings)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        pairs = new Dictionary<string, T>(settings.NameComparer);
        foreach (var pair in source.Where(p => !string.IsNullOrEmpty(p.Item1)))
        {
            pairs[pair.Item1] = pair.Item2;
        }
    }

    public TryGetResult TryMap(string token) =>
        pairs.TryGetValue(token, out var value) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;

#if NET8_0_OR_GREATER
    /// <summary>
    /// Converts the inner dictionary to a FrozenDictionary for faster read performance if reused.
    /// </summary>
    public DictionaryTokenValueContainer<T> Frozen()
    {
        pairs = pairs.ToFrozenDictionary();
        return this;
    }
#endif
}
