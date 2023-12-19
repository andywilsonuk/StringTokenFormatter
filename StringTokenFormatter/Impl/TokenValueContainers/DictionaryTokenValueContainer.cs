#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

public sealed class DictionaryTokenValueContainer<T> : ITokenValueContainer
{
    private IDictionary<string, T> pairs;
    private readonly ITokenValueContainerSettings settings;

    internal DictionaryTokenValueContainer(IEnumerable<(string TokenName, T Value)> source, ITokenValueContainerSettings settings)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        pairs = new Dictionary<string, T>(settings.NameComparer);
        foreach (var pair in source)
        {
            var (tokenName, value) = pair;
            if (tokenName == string.Empty) { throw new InvalidTokenNameException("Empty string cannot be used as token name"); }
            if (pairs.ContainsKey(tokenName)) { throw new InvalidTokenNameException($"The container already has a token with name '{tokenName}'"); }
            pairs.Add(tokenName, value);
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
