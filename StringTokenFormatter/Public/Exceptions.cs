﻿namespace StringTokenFormatter;

public abstract class StringTokenFormatterException : Exception
{
    public StringTokenFormatterException(string message) : base(message)
    {
    }

    public StringTokenFormatterException(string message, Exception ex) : base(message, ex)
    {
    }
}
public class UnresolvedTokenException : StringTokenFormatterException
{
    public UnresolvedTokenException(string message) : base(message)
    {
    }
}
public class ConditionTokenException : StringTokenFormatterException
{
    public ConditionTokenException(string message) : base(message)
    {
    }
}
public class TokenValueFormatException : StringTokenFormatterException
{
    public TokenValueFormatException(string message, Exception ex) : base(message, ex)
    {
    }
}
public class MissingValueConverterException : StringTokenFormatterException
{
    public MissingValueConverterException(string message) : base(message)
    {
    }
}
public class TokenContainerException : StringTokenFormatterException
{
    public TokenContainerException(string message) : base(message)
    {
    }
}
public class ParserException : StringTokenFormatterException
{
    public ParserException(string message) : base(message)
    {
    }
}