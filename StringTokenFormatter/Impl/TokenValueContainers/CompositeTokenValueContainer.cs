namespace StringTokenFormatter.Impl;

public sealed class CompositeTokenValueContainer : ITokenValueContainer
{
    private readonly ITokenValueContainer[] containers;
    private readonly ITokenValueContainerSettings settings;

    internal CompositeTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        ValidateArgs.AssertNotNull(containers, nameof(containers));
        this.containers = containers.Where(x => x is { }).ToArray();
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
