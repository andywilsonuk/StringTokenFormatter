namespace StringTokenFormatter.Impl;

public sealed class HierarchicalTokenValueContainer : ITokenValueContainer
{
    private readonly IHierarchicalTokenValueContainerSettings settings;
    private readonly string prefix;
    private readonly ITokenValueContainer container;

    internal HierarchicalTokenValueContainer(IHierarchicalTokenValueContainerSettings settings, string prefix, ITokenValueContainer container)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        this.prefix = ValidateArgs.AssertNotEmpty(prefix, nameof(prefix));
        this.container = ValidateArgs.AssertNotNull(container, nameof(container));
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
