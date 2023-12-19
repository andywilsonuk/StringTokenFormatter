#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace StringTokenFormatter.Impl;

public sealed class DictionaryTokenValueContainer<T> : ITokenValueContainer
{
    private readonly ITokenValueContainerSettings settings;
    private IDictionary<string, T> pairs;

    internal DictionaryTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<(string TokenName, T Value)> source)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        this.pairs = CreateDictionary(ValidateArgs.AssertNotNull(source, nameof(source)));
    }

    internal DictionaryTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<KeyValuePair<string, T>> source)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        this.pairs = CreateDictionary(ValidateArgs.AssertNotNull(source, nameof(source)).Select(x => (x.Key, x.Value)));
    }

    private Dictionary<string, T> CreateDictionary(IEnumerable<(string TokenName, T Value)> source)
    {
        var d = new Dictionary<string, T>(settings.NameComparer);
        foreach (var (tokenName, value) in source)
        {
            if (d.ContainsKey(tokenName)) { throw new TokenContainerException($"The container already has a token with name '{tokenName}'"); }
            d.Add(tokenName, value);
        }
        if (d.ContainsKey(string.Empty)) { throw new TokenContainerException("Empty string cannot be used as token name"); }
        if (d.Count == 0) { throw new TokenContainerException("The container is empty"); }
        return d;
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
