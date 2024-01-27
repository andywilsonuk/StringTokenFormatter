namespace StringTokenFormatter.Impl;

public sealed class ExpanderDataStore
{
    private readonly Dictionary<string, object> innerStore = new();

    public T Get<T>(string bucketName, string key) => (T)innerStore[FullKey(bucketName, key)];

    public void Set<T>(string bucketName, string key, T value) where T : notnull => innerStore[FullKey(bucketName, key)] = value;

    public bool Exists(string bucketName, string key) => innerStore.ContainsKey(FullKey(bucketName, key));

    private static string FullKey(string bucketName, string key) => $"{bucketName}_{key}";
}