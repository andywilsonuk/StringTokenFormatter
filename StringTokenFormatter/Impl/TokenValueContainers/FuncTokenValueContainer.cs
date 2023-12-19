namespace StringTokenFormatter.Impl;

public sealed class FuncTokenValueContainer<T> : ITokenValueContainer
{
    private readonly ITokenValueContainerSettings settings;
    private readonly Func<string, T> func;

    internal FuncTokenValueContainer(ITokenValueContainerSettings settings, Func<string, T> func)
    {
        this.settings = Guard.NotNull(settings, nameof(settings));
        this.func = Guard.NotNull(func, nameof(func));
    }

    public TryGetResult TryMap(string token)
    {
        var value = func(token);
        return settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
    }
}
