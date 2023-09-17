namespace StringTokenFormatter;

public class UnresolvedTokenException : Exception
{
    public UnresolvedTokenException(string? message) : base(message)
    {
    }
}
