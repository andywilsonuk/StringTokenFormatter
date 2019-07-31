using Xunit;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests {
    public abstract class TokenMarkerTestsBase {
        protected static void SingleInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        protected static void SingleIntegerInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        protected static void OpenEscapedCharacterYieldsReplacementInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        protected static void MissingTokenValueInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "$(two)", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        protected static void OpenEscapedCharacterYieldsNothingInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }


    }
}
