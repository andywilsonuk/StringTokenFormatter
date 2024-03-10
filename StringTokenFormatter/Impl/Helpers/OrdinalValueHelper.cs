namespace StringTokenFormatter.Impl;

public static class OrdinalValueHelper
{
    public static bool AreEqual(string a, string b) => string.Equals(a, b, StringComparison.Ordinal);
    public static bool StartsWith(string super, string sub) => super.StartsWith(sub, StringComparison.Ordinal);
    public static int? IndexOf(string super, string sub) => super.IndexOf(sub) switch
    {
        -1 => null,
        int r => r,
    };
}