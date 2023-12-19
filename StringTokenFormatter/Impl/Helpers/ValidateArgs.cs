namespace StringTokenFormatter.Impl;

internal static class ValidateArgs
{
    public static T AssertNotNull<T>(T source, string sourceName)
    {
        if (source == null) { throw new ArgumentNullException(sourceName); }        
        return source;
    }
}