using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringTokenFormatter
{
    public class DefaultTokenMatcher : ITokenMatcher
    {
        private readonly TokenMarkers markers;
        private readonly string segmentPattern;
        private readonly string tokenTriplePattern;

        public DefaultTokenMatcher(TokenMarkers tokenMarkers)
        {
            markers = tokenMarkers ?? throw new ArgumentNullException(nameof(tokenMarkers));
            string regexEscapedStartToken = Regex.Escape(markers.StartToken);
            string regexEscapedEndToken = Regex.Escape(markers.EndToken);
            string regexEscapedPaddingSeparator = Regex.Escape(",");
            string regexEscapedFormattingSeparator = Regex.Escape(":");
            segmentPattern = $"(.*?)({regexEscapedStartToken}[^{regexEscapedStartToken}{regexEscapedEndToken}]*?{regexEscapedEndToken})(.*?)";
            tokenTriplePattern = $"^([^{regexEscapedPaddingSeparator}{regexEscapedFormattingSeparator}]*){regexEscapedPaddingSeparator}?([^{regexEscapedFormattingSeparator}]*){regexEscapedFormattingSeparator }?(.*)$";
        }

        public DefaultTokenMatcher()
            : this(new DefaultTokenMarkers())
        {
        }

        public IEnumerable<IMatchingSegment> SplitSegments(string input)
        {
            string[] segments = Regex.Split(input, segmentPattern, RegexOptions.Singleline);
            foreach (string segment in segments.Where(s => !string.IsNullOrEmpty(s)))
            {
                string unescapedSegment = segment.Replace(markers.StartTokenEscaped, markers.StartToken);
                unescapedSegment = unescapedSegment.Replace(markers.EndTokenEscaped, markers.EndToken);
                if (segment.StartsWith(markers.StartToken) && segment.EndsWith(markers.EndToken))
                {
                    yield return ConvertTokenTripleToSegment(unescapedSegment);
                    continue;
                }
                yield return new TextMatchingSegment(unescapedSegment);
            }
        }

        private TokenMatchingSegment ConvertTokenTripleToSegment(string tokenTriple)
        {
            string tripleWithoutMarkers = tokenTriple.Remove(0, markers.StartToken.Length);
            tripleWithoutMarkers = tripleWithoutMarkers.Remove(tripleWithoutMarkers.Length - markers.EndToken.Length);

            string[] split = Regex.Split(tripleWithoutMarkers, tokenTriplePattern, RegexOptions.Singleline);
            return new TokenMatchingSegment(tokenTriple, split[1], split[2], split[3]);
        }
    }
}
