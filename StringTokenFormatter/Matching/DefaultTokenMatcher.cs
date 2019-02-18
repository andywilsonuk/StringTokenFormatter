using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringTokenFormatter
{
    public class DefaultTokenMatcher : ITokenMatcher {
        private static readonly string regexEscapedPaddingSeparator = Regex.Escape(",");
        private static readonly string regexEscapedFormattingSeparator = Regex.Escape(":");
        private static readonly string tokenTriplePattern = $"^([^{regexEscapedPaddingSeparator}{regexEscapedFormattingSeparator}]*){regexEscapedPaddingSeparator}?([^{regexEscapedFormattingSeparator}]*){regexEscapedFormattingSeparator }?(.*)$";
        private static readonly Regex tokenTripleRegex = new Regex(tokenTriplePattern, RegexOptions.Compiled | RegexOptions.Singleline);

        private readonly TokenMarkers markers;
        private readonly string segmentPattern;
        private readonly Regex segmentRegex;

        public DefaultTokenMatcher(TokenMarkers tokenMarkers) {
            markers = tokenMarkers ?? throw new ArgumentNullException(nameof(tokenMarkers));
            string regexEscapedStartToken = Regex.Escape(markers.StartToken);
            string regexEscapedEscapedStartToken = Regex.Escape(markers.StartTokenEscaped);
            string regexEscapedEndToken = Regex.Escape(markers.EndToken);
            segmentPattern = $"({regexEscapedEscapedStartToken})|({regexEscapedStartToken}[^ ]{{1}}.*?{regexEscapedEndToken})";
            segmentRegex = new Regex(segmentPattern, RegexOptions.Singleline | RegexOptions.Compiled);
        }

        public DefaultTokenMatcher()
            : this(new DefaultTokenMarkers()) {
        }

        public SegmentedString SplitSegments(string Input) {
            return new SegmentedString(SplitSegmentsInternal(Input));
        }

        private IEnumerable<IMatchingSegment> SplitSegmentsInternal(string input) {
            if (string.IsNullOrEmpty(input)) yield break;
            int index = 0;
            foreach (Match match in segmentRegex.Matches(input)) {
                string segment = match.Value;
                if (index != match.Index) {
                    string text = input.Substring(index, match.Index - index);
                    yield return new TextMatchingSegment(text);
                }
                if (segment == markers.StartTokenEscaped) {
                    yield return new TextMatchingSegment(markers.StartToken);
                } else if (!segment.StartsWith(markers.StartToken)) {
                    yield return new TextMatchingSegment(segment);
                } else {
                    int middleLength = segment.Length - markers.StartToken.Length - markers.EndToken.Length;
                    string tripleWithoutMarkers = segment.Substring(markers.StartToken.Length, middleLength);
                    string[] split = tokenTripleRegex.Split(tripleWithoutMarkers);
                    yield return new TokenMatchingSegment(segment, split[1], split[2], split[3]);
                }
                index = match.Index + match.Length;
            }
            if (index < input.Length) {
                yield return new TextMatchingSegment(input.Substring(index));
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

        public IEqualityComparer<string> TokenNameComparer => markers.TokenNameComparer;

        public IEnumerable<string> MatchedTokens(string input) {
            return SplitSegments(input).OfType<TokenMatchingSegment>().Select(x => x.Token);
        }
    }
}
