namespace StringTokenFormatter.Impl;

public static class InterpolatedStringParser
{
    public static InterpolatedString Parse(string source, IInterpolatedStringSettings settings)
    {
#if NET6_0_OR_GREATER
        return SpanInterpolatedStringParser.Parse(source, settings);
#else
        return RegexInterpolatedStringParser.Parse(source, settings);
#endif
    }
}