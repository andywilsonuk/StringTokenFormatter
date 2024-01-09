namespace StringTokenFormatter.Impl;

public sealed class ExpanderValueStore
{
    private readonly Dictionary<string, object> innerStore = new();

    public T Get<T>(string bucketName, string key, Func<T> defaultValue)
    {
        if (innerStore.TryGetValue(FullKey(bucketName, key), out var o))
        {
            return (T)o;
        }
        return defaultValue();
    }

    public void Set<T>(string bucketName, string key, T value) where T : notnull
    {
        innerStore[FullKey(bucketName, key)] = value;
    }

    private static string FullKey(string bucketName, string key) => $"{bucketName}_{key}";
}