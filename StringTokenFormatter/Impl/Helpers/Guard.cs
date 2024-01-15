namespace StringTokenFormatter.Impl;

internal static class Guard
{
    public static T NotNull<T>(T source, string sourceName)
    {
        if (source == null) { throw new ArgumentException("Argument cannot be null", sourceName); }        
        return source;
    }

    public static string NotEmpty(string source, string sourceName)
    {
        if (string.IsNullOrEmpty(source)) { throw new ArgumentException("String cannot be null or empty", sourceName); }        
        return source;
    }
}