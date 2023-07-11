namespace StringTokenFormatter.Impl;

public static class TokenValueContainerFactory
{
    public static TokenValueContainer<T> FromDictionary<T>(ITokenContainerSettings settings, IEnumerable<TokenValue<T>> source) => new(source, settings);
    public static ObjectTokenValueContainer<T> FromObject<T>(ITokenContainerSettings settings, T source) => new(source, settings);
    public static TokenValueContainer<T> FromSingle<T>(ITokenContainerSettings settings, string token, T value) => new(new TokenValue<T>[] { new(token, value) }, settings);
    public static FuncTokenValueContainer<T> FromFunc<T>(ITokenContainerSettings settings, Func<string, T> func) => new(func, settings);
    public static CompositeTokenValueContainer FromCombination(ITokenContainerSettings settings, IEnumerable<ITokenValueContainer> containers) => new(containers, settings);
    public static CompositeTokenValueContainer FromCombination(ITokenContainerSettings settings, params ITokenValueContainer[] containers) => new(containers, settings);
}
