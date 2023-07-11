﻿using System.Text;

namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container)
    {
        if (interpolatedString == null) { throw new ArgumentNullException(nameof(interpolatedString)); }
        if (container == null) { throw new ArgumentNullException(nameof(container)); }

        var sb = new StringBuilder();
        foreach (var segment in interpolatedString.Segments)
        {
            sb.Append(Evaluate(segment, container, interpolatedString.Settings));
        }
        return sb.ToString();
    }

    private static string Evaluate(InterpolatedStringSegment segment, ITokenValueContainer container, IIInterpolatedStringSettings settings)
    {
        var tokenSegment = segment as InterpolatedStringTokenSegment;
        if (tokenSegment == null) { return FormatValue(segment.Raw, null, null, settings.FormatProvider); }

        object? tokenValue = tokenSegment.Raw;

        var containerMatch = container.TryMap(tokenSegment.Token!);
        if (containerMatch.IsSuccess)
        {
            tokenValue = ConvertValue(tokenSegment, containerMatch.Value, settings);
        }
        else if (settings.UnresolvedTokenBehavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{tokenSegment.Token}' was not found within the container");
        }

        return FormatValue(tokenValue, tokenSegment.Padding, tokenSegment.Format, settings.FormatProvider);
    }

    private static object? ConvertValue(InterpolatedStringTokenSegment segment, object? value, IIInterpolatedStringSettings settings)
    {
        var tokenValuePair = new TokenValuePair(segment.Token!, value);
        var converter = settings.ValueConverters.Select(fn => fn(tokenValuePair)).FirstOrDefault(x => x.IsSuccess);
        return converter == default ? value : converter.Value;
    }

    private static string FormatValue(object? value, string? alignment, string? formatString, IFormatProvider formatProvider)
    {
        if (value is null) { return string.Empty; }

        bool isAlignmentEmpty = string.IsNullOrEmpty(alignment);
        bool isFormatStringEmpty = string.IsNullOrEmpty(formatString);

        if (isAlignmentEmpty && isFormatStringEmpty) { return value.ToString()!; }
        if (isAlignmentEmpty) { alignment = "0"; }
        if (isFormatStringEmpty) { formatString = "G"; }

        try
        {
            return string.Format(formatProvider, $"{{0,{alignment}:{formatString}}}", value);
        }
        catch (FormatException)
        {
            return value.ToString()!;
        }
    }
}
