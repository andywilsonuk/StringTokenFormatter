using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static partial class InterpolatedStringParser
{
    private const string paddingSeparator = ",";
    private const string formattingSeparator = ":";
    private const string tokenTriplePattern = $"^([^{paddingSeparator}{formattingSeparator}]*){paddingSeparator}?([^{formattingSeparator}]*){formattingSeparator}?(.*)$";
    private static readonly RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;

#if NET7_0_OR_GREATER
    [GeneratedRegex(tokenTriplePattern, RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetTokenTripleRegex();
# else
    private static readonly Regex tokenTripleRegex = new(tokenTriplePattern, regexOptions);
    private static Regex GetTokenTripleRegex() => tokenTripleRegex;
#endif

    public static InterpolatedString Parse(string source, IInterpolatedStringSettings settings)
    {
        if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

        string segmentPattern = GetRegexPattern(settings);
        var segments = ParseInternal(source, segmentPattern, settings);
        return new InterpolatedString(segments.ToList().AsReadOnly(), settings);
    }

    private static string GetRegexPattern(IInterpolatedStringSettings settings)
    {
        var (startToken, endToken, escapedStartToken) = settings.Syntax;
        var regexEscapedStartToken = Regex.Escape(startToken);
        var regexEscapedEscapedStartToken = Regex.Escape(escapedStartToken);
        var regexEscapedEndToken = Regex.Escape(endToken);
        return $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}.*?{regexEscapedEndToken})";
    }

    private static IEnumerable<InterpolatedStringSegment> ParseInternal(string source, string segmentPattern, IInterpolatedStringSettings settings)
    {
        if (string.IsNullOrEmpty(source)) yield break;
        var (startToken, endToken, escapedStartToken) = settings.Syntax;
        int index = 0;
        var matches = Regex.Matches(source, segmentPattern, regexOptions);
        foreach (var match in matches.Cast<Match>())
        {
            string segment = match.Value;
            int captureIndex = match.Index;
            if (index != captureIndex)
            {
                string text = source.Substring(index, match.Index - index);
                yield return new InterpolatedStringSegment(text);
            }
            if (segment == escapedStartToken)
            {
                yield return new InterpolatedStringSegment(startToken);
            }
            else if (!segment.StartsWith(startToken))
            {
                yield return new InterpolatedStringSegment(segment);
            }
            else
            {
                int middleLength = segment.Length - startToken.Length - endToken.Length;
                string tripleWithoutMarkers = segment.Substring(startToken.Length, middleLength);
                var split = GetTokenTripleRegex().Split(tripleWithoutMarkers);
                yield return new InterpolatedStringTokenSegment(segment, split[1], split[2], split[3]);
            }
            index = captureIndex + match.Length;
        }
        if (index < source.Length)
        {
            yield return new InterpolatedStringSegment(source.Substring(index));
        }
    }
}