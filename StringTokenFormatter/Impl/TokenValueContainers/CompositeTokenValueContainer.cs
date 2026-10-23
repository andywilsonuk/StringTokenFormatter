namespace StringTokenFormatter.Impl;

public sealed class CompositeTokenValueContainer : ITokenValueContainer
{
    private readonly ICompositeTokenValueContainerSettings settings;
    private readonly ITokenValueContainer[] containers;

    internal CompositeTokenValueContainer(ICompositeTokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        Guard.NotNull(containers, nameof(containers));
        this.containers = containers.Select(x => Guard.NotNull(x, $"Child container of {nameof(containers)} is null")).ToArray();
        Guard.NotEmpty(this.containers, nameof(containers));
    }

    public TryGetResult TryMap(string token) =>
        containers.Select(c => c.TryMap(token)).FirstOrDefault(value => value.IsSuccess && settings.TokenResolutionPolicy.Satisfies(value.Value));
}
