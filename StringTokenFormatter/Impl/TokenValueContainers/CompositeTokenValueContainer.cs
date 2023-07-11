using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

/// <summary>
/// This Value Container searches all child containers for the provided token value and returns the first value found. 
/// </summary>
public class CompositeTokenValueContainer : ITokenValueContainer
{
    private readonly ITokenValueContainer[] containers;
    private readonly ITokenContainerSettings settings;

    public CompositeTokenValueContainer(IEnumerable<ITokenValueContainer> containers, ITokenContainerSettings settings)
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
