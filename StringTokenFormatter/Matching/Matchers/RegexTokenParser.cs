using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringTokenFormatter {
    public class RegexTokenParser : ITokenParser {
        private static readonly string regexEscapedPaddingSeparator = Regex.Escape(",");
        private static readonly string regexEscapedFormattingSeparator = Regex.Escape(":");
        private static readonly string tokenTriplePattern = $"^([^{regexEscapedPaddingSeparator}{regexEscapedFormattingSeparator}]*){regexEscapedPaddingSeparator}?([^{regexEscapedFormattingSeparator}]*){regexEscapedFormattingSeparator }?(.*)$";
        private static readonly Regex tokenTripleRegex = new(tokenTriplePattern, RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly ITokenMarkers markers;
        private readonly Regex segmentRegex;

        public RegexTokenParser(ITokenMarkers? tokenMarkers = default) {
            markers = tokenMarkers ?? TokenMarkers.Default;
            var regexEscapedStartToken = Regex.Escape(markers.StartToken);
            var regexEscapedEscapedStartToken = Regex.Escape(markers.StartTokenEscaped);
            var regexEscapedEndToken = Regex.Escape(markers.EndToken);
            var segmentPattern = $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}[^ ]{{1}}.*?{regexEscapedEndToken})";
            segmentRegex = new Regex(segmentPattern, RegexOptions.Singleline | RegexOptions.Compiled);
        }



        public SegmentedString Parse(string input) {
            return new SegmentedString(ParseInternal(input));
        }

        private IEnumerable<ISegment> ParseInternal(string input) {
            if (string.IsNullOrEmpty(input)) yield break;
            int index = 0;
            foreach (Match match in segmentRegex.Matches(input)) {
                string segment = match.Value;
                if (index != match.Index) {
                    string text = input.Substring(index, match.Index - index);
                    yield return new StringSegment(text);
                }
                if (segment == markers.StartTokenEscaped) {
                    yield return new StringSegment(markers.StartToken);
                } else if (!segment.StartsWith(markers.StartToken)) {
                    yield return new StringSegment(segment);
                } else {
                    int middleLength = segment.Length - markers.StartToken.Length - markers.EndToken.Length;
                    string tripleWithoutMarkers = segment.Substring(markers.StartToken.Length, middleLength);
                    string[] split = tokenTripleRegex.Split(tripleWithoutMarkers);
                    yield return new TokenSegment(segment, split[1], split[2], split[3]);
                }
                index = match.Index + match.Length;
            }
            if (index < input.Length) {
                yield return new StringSegment(input.Substring(index));
            }
        }

        public string RemoveTokenMarkers(string token) {
            if (token.StartsWith(markers.StartToken) && !token.StartsWith(markers.StartTokenEscaped)) {
                string strippedToken = token.Remove(0, markers.StartToken.Length);

                if (token.EndsWith(markers.EndToken)) {
                    strippedToken = strippedToken.Remove(strippedToken.Length - markers.EndToken.Length);
                }
                return strippedToken;
            }
            return token;
        }

    }
}
