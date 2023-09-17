using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static class InterpolatedStringParser
{
    private static readonly string paddingSeparator = Regex.Escape(",");
    private static readonly string formattingSeparator = Regex.Escape(":");
    private static readonly string tokenTriplePattern = $"^([^{paddingSeparator}{formattingSeparator}]*){paddingSeparator}?([^{formattingSeparator}]*){formattingSeparator}?(.*)$";
    private static readonly Lazy<Regex> tokenTripleRegex = new(() => new(tokenTriplePattern, RegexOptions.Compiled | RegexOptions.Singleline));
    private static readonly List<(TokenSyntax Syntax, Regex Regex)> syntaxCache = new();
    private static readonly RegexOptions regexOptions = RegexOptions.None
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant
        | RegexOptions.ExplicitCapture
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled;

    public static InterpolatedString Parse(string input, IIInterpolatedStringSettings settings)
    {
        if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

        var segmentRegex = GetRegexFromCacheOrCreate(settings);
        var segments = ParseInternal(input, segmentRegex, settings);
        return new InterpolatedString(segments.ToList().AsReadOnly(), settings);
    }

    private static Regex GetRegexFromCacheOrCreate(IIInterpolatedStringSettings settings)
    {
        var existing = syntaxCache.FirstOrDefault(x => x.Syntax == settings.Syntax);

        if (!(existing == default)) { return existing.Regex; }

        var (startToken, endToken, escapedStartToken) = settings.Syntax;
        string segmentPattern = $"({Regex.Escape(startToken)})|({Regex.Escape(escapedStartToken)}.*?{Regex.Escape(endToken)})";
        Regex segmentRegex = new(segmentPattern, regexOptions);
        syntaxCache.Add((settings.Syntax, segmentRegex));
        return segmentRegex;
    }

    private static IEnumerable<InterpolatedStringSegment> ParseInternal(string input, Regex segmentRegex, IIInterpolatedStringSettings settings)
    {
        if (string.IsNullOrEmpty(input)) yield break;
        var (startToken, endToken, escapedStartToken) = settings.Syntax;
        int index = 0;
        foreach (Match match in segmentRegex.Matches(input).Cast<Match>())
        {
            string segment = match.Value;
            int captureIndex = match.Index;
            if (index != captureIndex)
            {
                string text = input.Substring(index, match.Index - index);
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
                var split = tokenTripleRegex.Value.Split(tripleWithoutMarkers);
                yield return new InterpolatedStringTokenSegment(segment, split[1], split[2], split[3]);
            }
            index = captureIndex + match.Length;
        }
        if (index < input.Length)
        {
            yield return new InterpolatedStringSegment(input.Substring(index));
        }
    }
}