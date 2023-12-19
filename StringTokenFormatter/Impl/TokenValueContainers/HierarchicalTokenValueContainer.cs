namespace StringTokenFormatter.Impl;

public sealed class HierarchicalTokenValueContainer : ITokenValueContainer
{
    private readonly string prefix;
    private readonly ITokenValueContainer container;
    private readonly IHierarchicalTokenValueContainerSettings settings;

    internal HierarchicalTokenValueContainer(IHierarchicalTokenValueContainerSettings settings, string prefix, ITokenValueContainer container)
    {
        if (prefix.Length == 0) { throw new ArgumentNullException(nameof(prefix)); }
        this.prefix = prefix;
        this.container = container ?? throw new ArgumentNullException(nameof(container));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token)
    {
        int prefixIndex = token.IndexOf(settings.HierarchicalDelimiter, StringComparison.Ordinal);
        if (prefixIndex == -1) { return default; }
        if (!settings.NameComparer.Equals(prefix, token.Substring(0, prefixIndex))) { return default; }

        string remainingToken = token.Substring(prefixIndex + settings.HierarchicalDelimiter.Length);
        return container.TryMap(remainingToken);
    }
}
