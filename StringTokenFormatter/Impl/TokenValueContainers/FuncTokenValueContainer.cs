﻿namespace StringTokenFormatter.Impl;

public sealed class FuncTokenValueContainer<T> : ITokenValueContainer
{
    private readonly ITokenValueContainerSettings settings;
    private readonly Func<string, T> func;

    internal FuncTokenValueContainer(ITokenValueContainerSettings settings, Func<string, T> func)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        this.func = ValidateArgs.AssertNotNull(func, nameof(func));
    }

    public TryGetResult TryMap(string token)
    {
        var value = func(token);
        return settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
    }
}
