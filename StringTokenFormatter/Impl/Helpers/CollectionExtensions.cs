namespace StringTokenFormatter.Impl;

internal static class CollectionExtensions
{
    public static void ForEach<T>(this IReadOnlyCollection<T> collection, Action<T> action)
    {
        foreach (T? item in collection)
        {
            action(item);
        }
    }

    public static void ForEach<T>(this IReadOnlyCollection<T> collection, Action<T> action, Func<bool> haltPreciate)
    {
        foreach (T? item in collection)
        {
            action(item);
            if (haltPreciate()) { break; }
        }
    }
}