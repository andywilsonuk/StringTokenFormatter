namespace StringTokenFormatter.Impl;

public sealed class HierarchicalTokenValueContainer : ITokenValueContainer
{
    private readonly IHierarchicalTokenValueContainerSettings settings;
    private readonly string prefix;
    private readonly ITokenValueContainer container;

    internal HierarchicalTokenValueContainer(IHierarchicalTokenValueContainerSettings settings, string prefix, ITokenValueContainer container)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        this.prefix = Guard.NotEmpty(prefix, nameof(prefix));
        this.container = Guard.NotNull(container, nameof(container));
        Guard.NotEmpty(settings.HierarchicalDelimiter, nameof(settings.HierarchicalDelimiter));
    }

    public TryGetResult TryMap(string token)
    {
        int? prefixIndex = OrdinalValueHelper.IndexOf(token, settings.HierarchicalDelimiter);
        if (prefixIndex == null) { return default; }
        if (!settings.NameComparer.Equals(prefix, token[..prefixIndex.Value])) { return default; }

        string remainingToken = token[(prefixIndex.Value + settings.HierarchicalDelimiter.Length)..];
        return container.TryMap(remainingToken);
    }
}
