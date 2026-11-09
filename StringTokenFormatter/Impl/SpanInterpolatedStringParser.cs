#if NET6_0_OR_GREATER
namespace StringTokenFormatter.Impl;

public static class SpanInterpolatedStringParser
{
    private const string commandPrefix = Constants.CommandPrefix;
    private const string pseudoPrefix = Constants.PseudoPrefix;

    public static InterpolatedString Parse(string source, IInterpolatedStringSettings settings)
    {
        Guard.NotNull(settings, nameof(settings)).Validate();
        if (string.IsNullOrEmpty(source))
        {
            return InterpolatedString.CreateEmpty(settings);
        }

        (string? startToken, string? endToken, string? escapedStartToken) = settings.Syntax;
        List<TokenMatch> matches = GetTokenMatches(source, startToken, endToken, escapedStartToken);
        IEnumerable<InterpolatedStringSegment> segments = ConvertToSegments(source, matches, startToken, endToken);
        return InterpolatedString.Create(segments, settings);
    }

    private static List<TokenMatch> GetTokenMatches(string source, string startToken, string endToken, string escapedStartToken)
    {
        List<TokenMatch> matches = [];
        ReadOnlySpan<char> sourceSpan = source.AsSpan();
        ReadOnlySpan<char> startSpan = startToken.AsSpan();
        ReadOnlySpan<char> endSpan = endToken.AsSpan();
        ReadOnlySpan<char> escapedStartSpan = escapedStartToken.AsSpan();
        int searchIndex = 0;

        while (searchIndex < sourceSpan.Length)
        {
            ReadOnlySpan<char> remaining = sourceSpan[searchIndex..];
            int startOffset = remaining.IndexOf(startSpan, StringComparison.Ordinal);
            int escapedStartOffset = remaining.IndexOf(escapedStartSpan, StringComparison.Ordinal);

            if (startOffset < 0 && escapedStartOffset < 0)
            {
                break;
            }

            int nextStartIndex = startOffset < 0 ? int.MaxValue : searchIndex + startOffset;
            int nextEscapedStartIndex = escapedStartOffset < 0 ? int.MaxValue : searchIndex + escapedStartOffset;
            if (nextEscapedStartIndex <= nextStartIndex)
            {
                matches.Add(new TokenMatch(nextEscapedStartIndex, escapedStartToken.Length, true));
                searchIndex = nextEscapedStartIndex + escapedStartToken.Length;
                continue;
            }

            int endSearchIndex = nextStartIndex + startToken.Length;
            int endOffset = sourceSpan[endSearchIndex..].IndexOf(endSpan, StringComparison.Ordinal);
            if (endOffset < 0)
            {
                searchIndex = nextStartIndex + 1;
                continue;
            }

            int endIndex = endSearchIndex + endOffset;
            matches.Add(new TokenMatch(nextStartIndex, endIndex + endToken.Length - nextStartIndex, false));
            searchIndex = endIndex + endToken.Length;
        }

        return matches;
    }

    private static IEnumerable<InterpolatedStringSegment> ConvertToSegments(string source, IEnumerable<TokenMatch> matches, string startToken, string endToken)
    {
        int index = 0;
        foreach (TokenMatch match in matches)
        {
            string segment = source.Substring(match.Index, match.Length);
            if (index != match.Index)
            {
                yield return new InterpolatedStringLiteralSegment(source[index..match.Index]);
            }

            if (match.IsEscaped)
            {
                yield return new InterpolatedStringLiteralSegment(startToken);
            }
            else if (!OrdinalValueHelper.StartsWith(segment, startToken))
            {
                yield return new InterpolatedStringLiteralSegment(segment);
            }
            else
            {
                yield return ParseComponents(segment, startToken, endToken);
            }

            index = match.Index + match.Length;
        }

        if (index < source.Length)
        {
            yield return new InterpolatedStringLiteralSegment(source[index..]);
        }
    }

    private static InterpolatedStringSegment ParseComponents(string segment, string startToken, string endToken)
    {
        ReadOnlySpan<char> components = segment.AsSpan(startToken.Length, segment.Length - startToken.Length - endToken.Length);
        ReadOnlySpan<char> prefix = ReadOnlySpan<char>.Empty;

        if (components.StartsWith(pseudoPrefix.AsSpan(), StringComparison.Ordinal))
        {
            prefix = pseudoPrefix.AsSpan();
            components = components[prefix.Length..];
        }
        else if (components.StartsWith(commandPrefix.AsSpan(), StringComparison.Ordinal))
        {
            prefix = commandPrefix.AsSpan();
            components = components[prefix.Length..];
        }

        int tokenEnd = components.IndexOfAny(',', ':');
        if (tokenEnd < 0)
        {
            tokenEnd = components.Length;
        }

        ReadOnlySpan<char> tokenName = components[..tokenEnd];
        ReadOnlySpan<char> remaining = components[tokenEnd..];
        ReadOnlySpan<char> alignment = ReadOnlySpan<char>.Empty;
        ReadOnlySpan<char> formatOrData = ReadOnlySpan<char>.Empty;

        if (!remaining.IsEmpty && remaining[0] == ',')
        {
            remaining = remaining[1..];
            int formatSeparator = remaining.IndexOf(':');
            if (formatSeparator < 0)
            {
                alignment = remaining;
                remaining = ReadOnlySpan<char>.Empty;
            }
            else
            {
                alignment = remaining[..formatSeparator];
                remaining = remaining[formatSeparator..];
            }
        }

        if (!remaining.IsEmpty && remaining[0] == ':')
        {
            formatOrData = remaining[1..];
        }

        if (prefix.SequenceEqual(commandPrefix.AsSpan()))
        {
            return new InterpolatedStringCommandSegment(segment, tokenName.ToString(), alignment.ToString(), formatOrData.ToString());
        }

        if (prefix.SequenceEqual(pseudoPrefix.AsSpan()) && !tokenName.IsEmpty)
        {
            return new InterpolatedStringPseudoTokenSegment(segment, pseudoPrefix + tokenName.ToString(), alignment.ToString(), formatOrData.ToString());
        }

        if (prefix.IsEmpty && !tokenName.IsEmpty)
        {
            return new InterpolatedStringTokenSegment(segment, tokenName.ToString(), alignment.ToString(), formatOrData.ToString());
        }

        throw new ParserException($"Unable to parse segment: {segment}");
    }

    private readonly record struct TokenMatch(int Index, int Length, bool IsEscaped);
}
#endif
