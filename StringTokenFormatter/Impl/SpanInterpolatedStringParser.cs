#if NET6_0_OR_GREATER
using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static class SpanInterpolatedStringParser
{
    private const string commandPrefix = Constants.CommandPrefix;
    private const string pseudoPrefix = Constants.PseudoPrefix;
    private const string paddingSeparator = ",";
    private const string formattingSeparator = ":";
    private const string tokenComponentsPattern = $"^({pseudoPrefix}|{commandPrefix})?([^{paddingSeparator}{formattingSeparator}]*){paddingSeparator}?([^{formattingSeparator}]*){formattingSeparator}?(.*)$";
    private static readonly RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;
    private static readonly Regex tokenComponentsRegex = new(tokenComponentsPattern, regexOptions);

    public static InterpolatedString Parse(string source, IInterpolatedStringSettings settings)
    {
        Guard.NotNull(settings, nameof(settings)).Validate();
        if (string.IsNullOrEmpty(source))
        {
            return new InterpolatedString([], settings);
        }

        var (startToken, endToken, escapedStartToken) = settings.Syntax;
        var matches = GetTokenMatches(source, startToken, endToken, escapedStartToken);
        var segments = ConvertToSegments(source, matches, startToken, endToken, escapedStartToken);
        return new InterpolatedString(segments.ToList().AsReadOnly(), settings);
    }

    private static List<TokenMatch> GetTokenMatches(string source, string startToken, string endToken, string escapedStartToken)
    {
        var matches = new List<TokenMatch>();
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

    private static IEnumerable<InterpolatedStringSegment> ConvertToSegments(
        string source,
        IEnumerable<TokenMatch> matches,
        string startToken,
        string endToken,
        string escapedStartToken)
    {
        int index = 0;
        foreach (var match in matches)
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
        string componentPartsString = segment[startToken.Length..^endToken.Length];
        string[] components = tokenComponentsRegex.Split(componentPartsString);
        InterpolatedStringTokenOnlySegment parsedSegment = components switch
        {
            [_, commandPrefix, string commandName, string tokenName, string data, _] => new InterpolatedStringCommandSegment(segment, commandName, tokenName, data),
            [_, pseudoPrefix, string tokenName, string alignment, string format, _] when tokenName != string.Empty => new InterpolatedStringPseudoTokenSegment(segment, pseudoPrefix + tokenName, alignment, format),
            [_, string tokenName, string alignment, string format, _] when tokenName != string.Empty => new InterpolatedStringTokenSegment(segment, tokenName, alignment, format),
            _ => throw new ParserException($"Unable to parse segment: {segment}"),
        };
        return parsedSegment;
    }

    private readonly record struct TokenMatch(int Index, int Length, bool IsEscaped);
}
#endif
