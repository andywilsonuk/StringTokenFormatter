using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public static class RegexInterpolatedStringParser
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
            return InterpolatedString.CreateEmpty(settings);
        }
        IEnumerable<Match> matches = GetRegexMatches(source, settings.Syntax);
        IEnumerable<InterpolatedStringSegment> segments = ConvertToSegments(source, matches, settings.Syntax);
        return InterpolatedString.Create(segments, settings);
    }

    private static IEnumerable<Match> GetRegexMatches(string source, TokenSyntax syntax)
    {
        (string? startToken, string? endToken, string? escapedStartToken) = syntax;
        string regexEscapedStartToken = Regex.Escape(startToken);
        string regexEscapedEscapedStartToken = Regex.Escape(escapedStartToken);
        string regexEscapedEndToken = Regex.Escape(endToken);
        string segmentPattern = $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}.*?{regexEscapedEndToken})";
        return Regex.Matches(source, segmentPattern, regexOptions).Cast<Match>();
    }

    private static IEnumerable<InterpolatedStringSegment> ConvertToSegments(string source, IEnumerable<Match> matches, TokenSyntax syntax)
    {
        (string? startToken, string? endToken, string? escapedStartToken) = syntax;
        int index = 0;
        foreach (Match match in matches)
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
}
