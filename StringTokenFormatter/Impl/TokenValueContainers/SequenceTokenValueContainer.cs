namespace StringTokenFormatter.Impl;

public sealed class SequenceTokenValueContainer : ITokenValueContainer, ISequenceTokenValueContainer
{
    private readonly IHierarchicalTokenValueContainerSettings settings;
    private readonly string prefix;
    private readonly List<object> values;

    internal SequenceTokenValueContainer(IHierarchicalTokenValueContainerSettings settings, string prefix, IEnumerable<object> values)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        this.prefix = Guard.NotEmpty(prefix, nameof(prefix));
        this.values = Guard.NotNull(values, nameof(values)).ToList();
        Guard.NotEmpty(settings.HierarchicalDelimiter, nameof(settings.HierarchicalDelimiter));
    }

    public TryGetResult TryMap(string token)
    {
        int prefixIndex = token.IndexOf(settings.HierarchicalDelimiter, StringComparison.Ordinal);
        string tokenPrefix = prefixIndex == -1 ? token : token[..prefixIndex]; 
        return settings.NameComparer.Equals(prefix, tokenPrefix) ? TryGetResult.Success(this) : default;
    }

    public TryGetResult TryMap(string token, int position)
    {
        int prefixStringIndex = token.IndexOf(settings.HierarchicalDelimiter, StringComparison.Ordinal);

        object? value = values[position - 1];
        if (prefixStringIndex == -1)
        {
            return TryGetResult.Success(value);
        }
        else if (value is ITokenValueContainer childContainer)
        {
            string remainingToken = prefixStringIndex == -1 ? string.Empty : token[(prefixStringIndex + settings.HierarchicalDelimiter.Length)..];
            return childContainer.TryMap(remainingToken);
        }
        return default;
    }

    public int Count => values.Count;
}
