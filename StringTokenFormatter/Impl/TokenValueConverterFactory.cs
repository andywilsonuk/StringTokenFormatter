using System.Linq.Expressions;

namespace StringTokenFormatter.Impl;

public delegate TryGetResult TokenValueConverter(object? value, string tokenName);

public static class TokenValueConverterFactory
{
    public static TokenValueConverter NullConverter() => (v, _n) => v is null ? TryGetResult.Success(null) : default;

    public static TokenValueConverter PrimitiveConverter() => (v, _n) => v is ValueType || v is string ? TryGetResult.Success(v) : default;

    public static TokenValueConverter LazyConverter<T>() => (v, _n) => v is Lazy<T> lazy ? TryGetResult.Success(lazy.Value) : default;

    public static TokenValueConverter FuncConverter<T>() => (v, _n) => v is Func<T> func ? TryGetResult.Success(func()) : default;
    public static TokenValueConverter FuncConverterNonGeneric() => (v, _n) =>
    {
        if (v == null) { return default;}
        var t = v.GetType();
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Func<>))
        {
            var invoker = t.GetMethod("Invoke");
            var value = invoker!.Invoke(v, null);
            return TryGetResult.Success(value);
        }
        return default;
    };

    public static TokenValueConverter TokenFuncConverter<T>() => (v, n) => v is Func<string, T> func ? TryGetResult.Success(func(n)) : default;
    
    public static TokenValueConverter ToStringConverter<T>() => (v, _n) => v is not null && v is T ? TryGetResult.Success(v.ToString()) : default;
}
