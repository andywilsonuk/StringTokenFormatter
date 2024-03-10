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
        int? prefixIndex = OrdinalValueHelper.IndexOf(token, settings.HierarchicalDelimiter);
        string tokenPrefix = prefixIndex == null ? token : token[..prefixIndex.Value];
        return settings.NameComparer.Equals(prefix, tokenPrefix) ? TryGetResult.Success(this) : default;
    }

    public TryGetResult TryMap(string token, int position)
    {
        int? prefixStringIndex = OrdinalValueHelper.IndexOf(token, settings.HierarchicalDelimiter);
        object? value = values[position - 1];
        if (prefixStringIndex == null) { return TryGetResult.Success(value); }
        if (value is ITokenValueContainer childContainer)
        {
            string remainingToken = token[(prefixStringIndex.Value + settings.HierarchicalDelimiter.Length)..];
            return childContainer.TryMap(remainingToken);
        }
        return default;
    }

    public int Count => values.Count;
}
