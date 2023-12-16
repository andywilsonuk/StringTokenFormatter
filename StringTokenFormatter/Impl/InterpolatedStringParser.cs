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
        if (string.IsNullOrEmpty(source))
        {
            return new InterpolatedString(Array.Empty<InterpolatedStringSegment>(), settings);
        }
        var matches = GetRegexMatches(source, settings.Syntax);
        var segments = ConvertToSegments(source, matches, settings.Syntax);
        return new InterpolatedString(segments.ToList().AsReadOnly(), settings);
    }

    private static IEnumerable<Match> GetRegexMatches(string source, TokenSyntax syntax)
    {
#if NET8_0_OR_GREATER
        var knownRegex = CommonTokenSyntaxRegexStore.GetRegex(syntax);
        if (knownRegex != null) { return knownRegex.Matches(source).Cast<Match>(); }
#endif
        var (startToken, endToken, escapedStartToken) = syntax;
        var regexEscapedStartToken = Regex.Escape(startToken);
        var regexEscapedEscapedStartToken = Regex.Escape(escapedStartToken);
        var regexEscapedEndToken = Regex.Escape(endToken);
        string segmentPattern = $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}.*?{regexEscapedEndToken})";
        return Regex.Matches(source, segmentPattern, regexOptions).Cast<Match>();
    }

    private static IEnumerable<InterpolatedStringSegment> ConvertToSegments(string source, IEnumerable<Match> matches, TokenSyntax syntax)
    {
        var (startToken, endToken, escapedStartToken) = syntax;
        int index = 0;
        foreach (var match in matches)
        {
            string segment = match.Value;
            int captureIndex = match.Index;
            int captureLength = match.Length;

            if (index != captureIndex)
            {
                yield return new InterpolatedStringSegment(source.Substring(index, captureIndex - index));
            }
            if (segment == escapedStartToken)
            {
                yield return new InterpolatedStringSegment(startToken);
            }
            else if (!segment.StartsWith(startToken, StringComparison.Ordinal))
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
            index = captureIndex + captureLength;
        }
        if (index < source.Length)
        {
            yield return new InterpolatedStringSegment(source.Substring(index));
        }
    }
}