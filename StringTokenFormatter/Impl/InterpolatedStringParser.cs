using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static partial class InterpolatedStringParser
{
    private const string commandPrefix = Constants.CommandPrefix;
    private const string pseudoPrefix = Constants.PseudoPrefix;
    private const string paddingSeparator = ",";
    private const string formattingSeparator = ":";
    private const string tokenComponentsPattern = $"^({pseudoPrefix}|{commandPrefix})?([^{paddingSeparator}{formattingSeparator}]*){paddingSeparator}?([^{formattingSeparator}]*){formattingSeparator}?(.*)$";
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
            else if (!OrdinalValueHelper.StartsWith(segment, startToken))
            {
                yield return new InterpolatedStringLiteralSegment(segment);
            }
            else
            {
                yield return ParseComponents(segment, startToken, endToken);
            }
            index = captureIndex + captureLength;
        }
        if (index < source.Length)
        {
            yield return new InterpolatedStringLiteralSegment(source[index..]);
        }
    }

    private static InterpolatedStringSegment ParseComponents(string segment, string startToken, string endToken)
    {
        string componentPartsString = segment[startToken.Length..^endToken.Length];
        string[] components = GetTokenComponentsRegex().Split(componentPartsString);
        InterpolatedStringTokenOnlySegment parsedSegment = components switch
        {
            [_, commandPrefix, string commandName, string tokenName, string data, _] => new InterpolatedStringCommandSegment(segment, commandName, tokenName, data),
            [_, pseudoPrefix, string tokenName, string alignment, string format, _] when tokenName != string.Empty => new InterpolatedStringPseudoTokenSegment(segment, pseudoPrefix + tokenName, alignment, format),
            [_, string tokenName, string alignment, string format, _] when tokenName != string.Empty => new InterpolatedStringTokenSegment(segment, tokenName, alignment, format),
            _ => throw new ParserException($"Unable to parse segment: {segment}"),
        };
        return parsedSegment;
    }
}