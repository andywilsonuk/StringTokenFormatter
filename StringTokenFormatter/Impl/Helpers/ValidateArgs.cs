namespace StringTokenFormatter.Impl;

internal static class ValidateArgs
{
    public static T AssertNotNull<T>(T source, string sourceName)
    {
        if (source == null) { throw new ArgumentNullException(sourceName); }        
        return source;
    }

    public static string AssertNotEmpty(string source, string sourceName)
    {
        if (string.IsNullOrEmpty(source)) { throw new ArgumentNullException(sourceName, "String cannot be null or empty"); }        
        return source;
    }
}