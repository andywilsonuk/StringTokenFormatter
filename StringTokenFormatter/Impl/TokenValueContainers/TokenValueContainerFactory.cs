namespace StringTokenFormatter.Impl;

public static class TokenValueContainerFactory
{
    public static DictionaryTokenValueContainer<T> FromPairs<T>(ITokenValueContainerSettings settings, IEnumerable<KeyValuePair<string, T>> source) => new(source, settings);
    public static DictionaryTokenValueContainer<T> FromTuples<T>(ITokenValueContainerSettings settings, IEnumerable<(string, T)> source) => new(source, settings);
    public static ObjectTokenValueContainer<T> FromObject<T>(ITokenValueContainerSettings settings, T source) => new(source, settings);
    public static SingleTokenValueContainer<T> FromSingle<T>(ITokenValueContainerSettings settings, string token, T value) => new(token, value, settings);
    public static FuncTokenValueContainer<T> FromFunc<T>(ITokenValueContainerSettings settings, Func<string, T> func) => new(func, settings);
    public static CompositeTokenValueContainer FromCombination(ITokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers) => new(containers, settings);
    public static CompositeTokenValueContainer FromCombination(ITokenValueContainerSettings settings, params ITokenValueContainer[] containers) => new(containers, settings);
    public static HierarchicalTokenValueContainer FromHierarchical(IHierarchicalTokenValueContainerSettings settings, string prefix, ITokenValueContainer container) => new(prefix, container, settings);
}
