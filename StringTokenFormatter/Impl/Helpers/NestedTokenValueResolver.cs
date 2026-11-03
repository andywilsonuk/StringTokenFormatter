namespace StringTokenFormatter.Impl;

internal static class NestedTokenValueResolver
{
    public static bool IsEnabled(ITokenValueContainerSettings settings) =>
        settings is INestedTokenValueContainerSettings nestedSettings
        && nestedSettings.ResolveNestedValues == NestedValueResolution.Nested;

    public static string GetDelimiter(ITokenValueContainerSettings settings)
    {
        string delimiter = settings is IHierarchicalTokenValueContainerSettings hierarchicalSettings
            ? hierarchicalSettings.HierarchicalDelimiter
            : ".";
        if (IsEnabled(settings)) { Guard.NotEmpty(delimiter, nameof(delimiter)); }
        return delimiter;
    }

    public static TryGetResult TryMapSegments(string token, int segmentStart, object? rootValue, string delimiter, StringComparer nameComparer)
    {
        object? current = rootValue;
        while (current is not null)
        {
            int delimiterIndex = token.IndexOf(delimiter, segmentStart, StringComparison.Ordinal);
            int segmentLength = (delimiterIndex < 0 ? token.Length : delimiterIndex) - segmentStart;
            string segment = token.Substring(segmentStart, segmentLength);
            if (!NestedPropertyCache.TryGetChildValue(current, segment, nameComparer, out current)) { return default; }
            if (delimiterIndex < 0) { return TryGetResult.Success(current); }
            segmentStart = delimiterIndex + delimiter.Length;
        }
        return default;
    }
}
