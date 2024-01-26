namespace StringTokenFormatter.Impl;

public sealed class SequenceTokenValueContainer : ITokenValueContainer, ISequenceTokenValueContainer
{
    private readonly IHierarchicalTokenValueContainerSettings settings;
    private readonly string prefix;
    private readonly List<object> values;

    public SequenceTokenValueContainer(IHierarchicalTokenValueContainerSettings settings, string prefix, IEnumerable<object> values)
    {
        this.settings = Guard.NotNull(settings, nameof(settings));
        this.prefix = Guard.NotEmpty(prefix, nameof(prefix));
        this.values = Guard.NotNull(values, nameof(values)).ToList();
        Guard.NotEmpty(settings.HierarchicalDelimiter, nameof(settings.HierarchicalDelimiter));
    }

    public TryGetResult TryMap(string token)
    {
        int prefixIndex = token.IndexOf(settings.HierarchicalDelimiter, StringComparison.Ordinal);
        string tokenPrefix = prefixIndex == -1 ? token : token.Substring(0, prefixIndex); 
        return settings.NameComparer.Equals(prefix, tokenPrefix) ? TryGetResult.Success(this) : default;
    }

    public int Count => values.Count;

    public TryGetResult TryMapForIndex(string token, int index)
    {
        int prefixIndex = token.IndexOf(settings.HierarchicalDelimiter, StringComparison.Ordinal);

        object? value = values[index];
        if (value is ITokenValueContainer childContainer)
        {
            string remainingToken = prefixIndex == -1 ? string.Empty : token.Substring(prefixIndex + settings.HierarchicalDelimiter.Length);
            return childContainer.TryMap(remainingToken);
        }
        else if (prefixIndex == -1)
        {
            return TryGetResult.Success(value);
        }
        return default;
    }
}
