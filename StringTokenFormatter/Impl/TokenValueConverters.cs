namespace StringTokenFormatter.Impl;

public delegate TryGetResult TokenValueConverter(object? value, string tokenName);

public static class TokenValueConverters
{
    public static TokenValueConverter NullConverter() => (v, _n) => v is null ? TryGetResult.Success(null) : default;

    public static TokenValueConverter PrimitiveConverter() => (v, _n) => v is ValueType || v is string ? TryGetResult.Success(v) : default;

    public static TokenValueConverter LazyConverter<T>() => (v, _n) => v is Lazy<T> lazy ? TryGetResult.Success(lazy.Value) : default;

    public static TokenValueConverter FuncConverter<T>() => (v, _n) => v is Func<T> func ? TryGetResult.Success(func()) : default;

    public static TokenValueConverter TokenFuncConverter<T>() => (v, n) => v is Func<string, T> func ? TryGetResult.Success(func(n)) : default;
    
    public static TokenValueConverter ToStringConverter<T>() => (v, _n) => v is not null && v is T ? TryGetResult.Success(v.ToString()) : default;
}
