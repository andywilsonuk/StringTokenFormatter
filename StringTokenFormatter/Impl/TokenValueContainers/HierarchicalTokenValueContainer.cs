namespace StringTokenFormatter.Impl;

/// <summary>
/// This Value Container attempts to match the prefix to the supplied token and passes the remainder to the inner container for matching. 
/// </summary>
public class HierarchicalTokenValueContainer : ITokenValueContainer
{
    private readonly string prefix;
    private readonly ITokenValueContainer container;
    private readonly IHierarchicalTokenValueContainerSettings settings;

    public HierarchicalTokenValueContainer(string prefix, ITokenValueContainer container, IHierarchicalTokenValueContainerSettings settings)
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
