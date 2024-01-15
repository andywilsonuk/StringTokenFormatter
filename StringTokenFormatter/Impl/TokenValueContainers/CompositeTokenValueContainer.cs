﻿namespace StringTokenFormatter.Impl;

public sealed class CompositeTokenValueContainer : ITokenValueContainer
{
    private readonly ITokenValueContainerSettings settings;
    private readonly ITokenValueContainer[] containers;

    internal CompositeTokenValueContainer(ITokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers)
    {
        this.settings = Guard.NotNull(settings, nameof(settings));
        Guard.NotNull(containers, nameof(containers));
        this.containers = containers.Select(x => Guard.NotNull(x, $"Child container of {nameof(containers)} is null")).ToArray();
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
