namespace StringTokenFormatter.Impl;

public sealed class CompositeTokenValueContainer : ITokenValueContainer
{
    private readonly ICompositeTokenValueContainerSettings settings;
    private readonly ITokenValueContainer[] containers;

    internal CompositeTokenValueContainer(ICompositeTokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        Guard.NotNull(containers, nameof(containers));
        this.containers = [.. containers.Select(x => Guard.NotNull(x, $"Child container of {nameof(containers)} is null"))];
        Guard.NotEmpty(this.containers, nameof(containers));
    }

    public TryGetResult TryMap(string token)
    {
        foreach (ITokenValueContainer container in containers)
        {
            TryGetResult value = container.TryMap(token);
            if (value.IsSuccess && settings.TokenResolutionPolicy.Satisfies(value.Value))
            {
                return value;
            }
        }
        return default;
    }
}
