namespace StringTokenFormatter.Impl;

internal static class CollectionExtensions
{
    extension<T>(IReadOnlyCollection<T> collection)
    {
        public void ForEach(Action<T> action)
        {
            foreach (T? item in collection)
            {
                action(item);
            }
        }

        public void ForEach(Action<T> action, Func<bool> haltPreciate)
        {
            foreach (T? item in collection)
            {
                action(item);
                if (haltPreciate()) { break; }
            }
        }
    }
}