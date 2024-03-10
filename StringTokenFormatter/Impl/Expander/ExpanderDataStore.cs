namespace StringTokenFormatter.Impl;

public sealed class ExpanderDataStore
{
    private readonly Dictionary<string, object> innerStore = [];

    public T Get<T>(string key) => (T)innerStore[key];
    public void Set<T>(string key, T value) where T : notnull => innerStore[key] = value;
}