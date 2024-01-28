namespace StringTokenFormatter.Impl;

public sealed class HierarchicalTokenValueContainer : ITokenValueContainer
{
    private readonly IHierarchicalTokenValueContainerSettings settings;
    private readonly string prefix;
    private readonly ITokenValueContainer container;

    internal HierarchicalTokenValueContainer(IHierarchicalTokenValueContainerSettings settings, string prefix, ITokenValueContainer container)
    {
        this.settings = Guard.NotNull(settings, nameof(settings));
        this.prefix = Guard.NotEmpty(prefix, nameof(prefix));
        this.container = Guard.NotNull(container, nameof(container));
        Guard.NotEmpty(settings.HierarchicalDelimiter, nameof(settings.HierarchicalDelimiter));
    }

    public TryGetResult TryMap(string token)
    {
        int prefixIndex = token.IndexOf(settings.HierarchicalDelimiter, StringComparison.Ordinal);
        if (prefixIndex == -1) { return default; }
        if (!settings.NameComparer.Equals(prefix, token[..prefixIndex])) { return default; }

        string remainingToken = token[(prefixIndex + settings.HierarchicalDelimiter.Length)..];
        return container.TryMap(remainingToken);
    }
}
