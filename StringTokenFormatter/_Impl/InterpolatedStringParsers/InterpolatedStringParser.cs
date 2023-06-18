using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

internal class InterpolatedStringParserImpl : IInterpolatedStringParser {
    private static readonly string regexEscapedPaddingSeparator = Regex.Escape(",");
    private static readonly string regexEscapedFormattingSeparator = Regex.Escape(":");
    private static readonly string tokenTriplePattern = $"^([^{regexEscapedPaddingSeparator}{regexEscapedFormattingSeparator}]*){regexEscapedPaddingSeparator}?([^{regexEscapedFormattingSeparator}]*){regexEscapedFormattingSeparator }?(.*)$";
    private static readonly Regex tokenTripleRegex = new(tokenTriplePattern, RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly ITokenSyntax markers;
    private readonly Regex segmentRegex;

    public InterpolatedStringParserImpl(ITokenSyntax tokenMarkers) {
        markers = tokenMarkers;

        var regexEscapedStartToken = Regex.Escape(markers.StartToken);
        var regexEscapedEscapedStartToken = Regex.Escape(markers.StartTokenEscaped);
        var regexEscapedEndToken = Regex.Escape(markers.EndToken);
        //var segmentPattern = $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}[^ ]{{1}}.*?{regexEscapedEndToken})";
        var segmentPattern = $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}.*?{regexEscapedEndToken})";

        var options = RegexOptions.None
            | RegexOptions.Singleline
            | RegexOptions.Compiled
            | RegexOptions.CultureInvariant
            | RegexOptions.ExplicitCapture
            | RegexOptions.IgnorePatternWhitespace
            ;

        segmentRegex = new Regex(segmentPattern, options);
    }



    public IInterpolatedString Parse(string input) {
        var Segments = ParseInternal(input);

        return new InterpolatedStringImpl(Segments);
    }

    private IEnumerable<IInterpolatedStringSegment> ParseInternal(string input) {
        if (string.IsNullOrEmpty(input)) yield break;
        int index = 0;
        foreach (Match match in segmentRegex.Matches(input)) {
            string segment = match.Value;
            if (index != match.Index) {
                string text = input.Substring(index, match.Index - index);
                yield return new InterpolatedStringSegmentLiteralImpl(text);
            }
            if (segment == markers.StartTokenEscaped) {
                yield return new InterpolatedStringSegmentLiteralImpl(markers.StartToken);
            } else if (!segment.StartsWith(markers.StartToken)) {
                yield return new InterpolatedStringSegmentLiteralImpl(segment);
            } else {
                int middleLength = segment.Length - markers.StartToken.Length - markers.EndToken.Length;
                string tripleWithoutMarkers = segment.Substring(markers.StartToken.Length, middleLength);
                string[] split = tokenTripleRegex.Split(tripleWithoutMarkers);
                yield return new InterpolatedStringSegmentTokenImpl(segment, split[1], split[2], split[3]);
            }
            index = match.Index + match.Length;
        }
        if (index < input.Length) {
            yield return new InterpolatedStringSegmentLiteralImpl(input.Substring(index));
        }
    }

}
