namespace StringTokenFormatter.Impl;

public sealed class CompositeTokenValueContainer : ITokenValueContainer
{
    private readonly ITokenValueContainer[] containers;
    private readonly ITokenValueContainerSettings settings;

    internal CompositeTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers)
    {
        if (containers == null) { throw new ArgumentNullException(nameof(containers)); }
        this.containers = containers.Where(x => x is { }).ToArray();
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token)
    {
        foreach (var container in containers)
        {
            var value = container.TryMap(token);
            if (value.IsSuccess && settings.TokenResolutionPolicy.Satisfies(value)) { return value; }
        }
        return default;
    }
}
