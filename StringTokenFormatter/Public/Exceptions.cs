namespace StringTokenFormatter;

public abstract class StringTokenFormatterException : Exception
{
    public StringTokenFormatterException(string message) : base(message)
    {
    }

    public StringTokenFormatterException(string message, Exception ex) : base(message, ex)
    {
    }
}
public class TokenContainerException(string message) : StringTokenFormatterException(message)
{
}
public class ParserException(string message) : StringTokenFormatterException(message)
{
}
public class ExpanderException(string message) : StringTokenFormatterException(message)
{
}
public class UnresolvedTokenException(string message) : StringTokenFormatterException(message)
{
}
public class MissingValueConverterException(string message) : StringTokenFormatterException(message)
{
}
public class TokenValueFormatException : StringTokenFormatterException
{
    public TokenValueFormatException(string message, Exception ex) : base(message, ex)
    {
    }

    public TokenValueFormatException(string message) : base(message)
    {
    }
}