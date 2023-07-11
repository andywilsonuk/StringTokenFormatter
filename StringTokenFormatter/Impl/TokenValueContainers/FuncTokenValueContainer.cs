namespace StringTokenFormatter.Impl;

public class FuncTokenValueContainer<T> : ITokenValueContainer
{
    private readonly Func<string, T> func;
    private readonly ITokenContainerSettings settings;

    public FuncTokenValueContainer(Func<string, T> func, ITokenContainerSettings settings)
    {
        this.func = func ?? throw new ArgumentNullException(nameof(func));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token) =>
        func(token) is { } value && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
