namespace StringTokenFormatter.Impl;

public delegate TryGetResult TokenValueConverter(object? value, string tokenName);

public static class TokenValueConverters
{
    public static TokenValueConverter FromNull() => (v, _n) => v is null ? TryGetResult.Success(null) : default;

    public static TokenValueConverter FromPrimitives() => (v, _n) => v is ValueType || v is string ? TryGetResult.Success(v) : default;

    public static TokenValueConverter FromLazy<T>() => (v, _n) => v is Lazy<T> lazy ? TryGetResult.Success(lazy.Value) : default;

    public static TokenValueConverter FromFunc<T>() => (v, _n) => v is Func<T> func ? TryGetResult.Success(func()) : default;

    public static TokenValueConverter FromTokenFunc<T>() => (v, n) => v is Func<string, T> func ? TryGetResult.Success(func(n)) : default;
}
