namespace StringTokenFormatter.Impl;

public record TokenValuePair(string Token, object? Value);

public static class TokenValueConverters
{
    public static Func<TokenValuePair, TryGetResult> FromNull() =>
        a => a.Value == null ? TryGetResult.Success(null) : default;

    public static Func<TokenValuePair, TryGetResult> FromPrimitives() =>
        a => a.Value != null && (a.Value is string || a.Value.GetType().IsValueType) ? TryGetResult.Success(a.Value) : default;

    public static Func<TokenValuePair, TryGetResult> FromLazy<T>() =>
        a => a.Value is Lazy<T> lazy ? TryGetResult.Success(lazy.Value) : default;

    public static Func<TokenValuePair, TryGetResult> FromFunc<T>() =>
        a => a.Value is Func<T> func ? TryGetResult.Success(func()) : default;

    public static Func<TokenValuePair, TryGetResult> FromTokenFunc<T>() =>
        a => a.Value is Func<string, T> func ? TryGetResult.Success(func(a.Token)) : default;
}
