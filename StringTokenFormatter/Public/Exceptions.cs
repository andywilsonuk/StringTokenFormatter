namespace StringTokenFormatter;

public abstract class StringTokenFormatterException : Exception
{
    public StringTokenFormatterException(string? message) : base(message)
    {
    }
}

public class UnresolvedTokenException : StringTokenFormatterException
{
    public UnresolvedTokenException(string? message) : base(message)
    {
    }
}
