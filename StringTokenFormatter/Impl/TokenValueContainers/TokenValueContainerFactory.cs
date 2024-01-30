namespace StringTokenFormatter.Impl;

public static class TokenValueContainerFactory
{
    /// <summary>
    /// Uses key/value pairs for token matching.
    /// </summary>
    public static DictionaryTokenValueContainer<T> FromPairs<T>(ITokenValueContainerSettings settings, IEnumerable<KeyValuePair<string, T>> source) => new(settings, source);
    /// <summary>
    /// Tuple key/value pairs for token matching.
    /// </summary>
    public static DictionaryTokenValueContainer<T> FromTuples<T>(ITokenValueContainerSettings settings, IEnumerable<(string TokenName, T Value)> source) => new(settings, source);
    /// <summary>
    /// Token matching provided by properties exposed by {T} (but not any members on derived classes).
    /// </summary>
    public static ObjectTokenValueContainer<T> FromObject<T>(ITokenValueContainerSettings settings, T source) where T : class => new(settings, source);
    /// <summary>
    /// Where a single token name and value is required.
    /// </summary>
    public static SingleTokenValueContainer<T> FromSingle<T>(ITokenValueContainerSettings settings, string token, T value) where T : notnull => new(settings, token, value);
    /// <summary>
    /// Delegates token matching to the func.
    /// </summary>
    public static FuncTokenValueContainer<T> FromFunc<T>(ITokenValueContainerSettings settings, Func<string, T> func) => new(settings, func);
    /// <summary>
    /// Searches child containers in order added for the first valid token value. 
    /// </summary>
    public static CompositeTokenValueContainer FromCombination(ITokenValueContainerSettings settings, IEnumerable<ITokenValueContainer> containers) => new(settings, containers);
    /// <summary>
    /// Searches child containers in order added for the first valid token value. 
    /// </summary>
    public static CompositeTokenValueContainer FromCombination(ITokenValueContainerSettings settings, params ITokenValueContainer[] containers) => new(settings, containers);
    /// <summary>
    /// Attempts to match the prefix to the supplied token and passes the remaining token name to the inner container for matching. 
    /// </summary>
    public static HierarchicalTokenValueContainer FromHierarchical(IHierarchicalTokenValueContainerSettings settings, string prefix, ITokenValueContainer container) => new(settings, prefix, container);
    /// <summary>
    /// Creates a container of values that can be used with `LoopBlockCommands`. 
    /// </summary>
    public static SequenceTokenValueContainer FromSequence<T>(IHierarchicalTokenValueContainerSettings settings, string token, IEnumerable<T> values) where T : notnull => new(settings, token, values.Cast<object>());
    /// <summary>
    /// Creates a container of containers that can be used with `LoopBlockCommands`. 
    /// </summary>
    public static SequenceTokenValueContainer FromSequence(IHierarchicalTokenValueContainerSettings settings, string token, IEnumerable<ITokenValueContainer> values) => new(settings, token, values);
}
