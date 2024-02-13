using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static partial class InterpolatedStringParser
{
    private const string commandPrefix = ":";
    private const string paddingSeparator = ",";
    private const string formattingSeparator = ":";
    private const string tokenComponentsPattern = $"^({commandPrefix}{{0,2}})?([^{paddingSeparator}{formattingSeparator}]*){paddingSeparator}?([^{formattingSeparator}]*){formattingSeparator}?(.*)$";
    private static readonly RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;

#if NET8_0_OR_GREATER
    [GeneratedRegex(tokenComponentsPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetTokenComponentsRegex();
# else
    private static readonly Regex tokenComponentsRegex = new(tokenComponentsPattern, regexOptions);
    private static Regex GetTokenComponentsRegex() => tokenComponentsRegex;
#endif

    public static InterpolatedString Parse(string source, IInterpolatedStringSettings settings)
    {
        Guard.NotNull(settings, nameof(settings)).Validate();
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
                yield return new InterpolatedStringLiteralSegment(source[index..captureIndex]);
            }
            if (segment == escapedStartToken)
            {
                yield return new InterpolatedStringLiteralSegment(startToken);
            }
            else if (!segment.StartsWith(startToken, StringComparison.Ordinal))
            {
                yield return new InterpolatedStringLiteralSegment(segment);
            }
            else
            {
                int middleLength = segment.Length - startToken.Length - endToken.Length;
                string componentPartsString = segment.Substring(startToken.Length, middleLength);
                var split = GetTokenComponentsRegex().Split(componentPartsString);
                bool isCommand = commandPrefix.Equals(split[1], StringComparison.Ordinal);

                if (isCommand)
                {
                    if (split[2].Length == 0) { throw new ParserException($"Blank token marker in command matched: {segment}"); }
                    yield return new InterpolatedStringCommandSegment(segment, split[2], split[3], split[4]);
                }
                else
                {
                    if (split[2].Length == 0) { throw new ParserException($"Blank token marker matched: {segment}"); }
                    yield return new InterpolatedStringTokenSegment(segment, split[1] + split[2], split[3], split[4]);
                }
            }
            index = captureIndex + captureLength;
        }
        if (index < source.Length)
        {
            yield return new InterpolatedStringLiteralSegment(source[index..]);
        }
    }
}