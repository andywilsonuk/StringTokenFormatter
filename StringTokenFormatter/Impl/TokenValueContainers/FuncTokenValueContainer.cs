namespace StringTokenFormatter.Impl;

public class FuncTokenValueContainer<T> : ITokenValueContainer
{
    private readonly Func<string, T> func;
    private readonly ITokenValueContainerSettings settings;

    public FuncTokenValueContainer(Func<string, T> func, ITokenValueContainerSettings settings)
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
