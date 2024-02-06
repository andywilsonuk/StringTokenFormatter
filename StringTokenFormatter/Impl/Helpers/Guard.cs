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

     public static IReadOnlyCollection<T> NotEmpty<T>(IReadOnlyCollection<T> source, string sourceName)
    {
        if (source.Count == 0) { throw new ArgumentException("Collection cannot be null or empty", sourceName); }        
        return source;
    }

    public static T IsDefined<T>(T source, string sourceName) where T : Enum
    {
        if(!Enum.IsDefined(typeof(T), source)) { throw new ArgumentException($"Specified value is not defined in enum", sourceName); }       
        return source;
    }
}