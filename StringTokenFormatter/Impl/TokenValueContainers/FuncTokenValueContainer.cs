namespace StringTokenFormatter.Impl;

public sealed class FuncTokenValueContainer<T> : ITokenValueContainer
{
    private readonly Func<string, T> func;
    private readonly ITokenValueContainerSettings settings;

    internal FuncTokenValueContainer(ITokenValueContainerSettings settings, Func<string, T> func)
    {
        this.func = func ?? throw new ArgumentNullException(nameof(func));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token)
    {
        var value = func(token);
        return settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
    }
}
