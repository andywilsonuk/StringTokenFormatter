namespace StringTokenFormatter.Impl;

public sealed class FuncTokenValueContainer<T> : ITokenValueContainer
{
    private readonly Func<string, T> func;

    internal FuncTokenValueContainer(ITokenValueContainerSettings _, Func<string, T> func)
    {
        this.func = Guard.NotNull(func, nameof(func));
    }

    public TryGetResult TryMap(string token) => TryGetResult.Success(func(token));
}
