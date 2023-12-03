namespace StringTokenFormatter.Impl;

/// <summary>
/// This Value Container uses key/value pairs for token matching.
/// </summary>
public class DictionaryTokenValueContainer<T> : ITokenValueContainer
{
    private readonly Dictionary<string, T> pairs;
    private readonly ITokenValueContainerSettings settings;

    public DictionaryTokenValueContainer(IEnumerable<KeyValuePair<string, T>> source, ITokenValueContainerSettings settings)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        pairs = new Dictionary<string, T>(settings.NameComparer);
        foreach (var pair in source.Where(p => !string.IsNullOrEmpty(p.Key)))
        {
            pairs[pair.Key] = pair.Value;
        }
    }

    public TryGetResult TryMap(string token) =>
        pairs.TryGetValue(token, out var value) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
