using Xunit;
using System.Collections.Generic;
using Moq;
using System;

namespace StringTokenFormatter.Tests {
    public class AlternatveTokenMarkerTests {
        [Fact]
        public void Single() {
            SingleInternal(CurlyTokenMarkers.Instance, "first {two} third", "first second third");
        }

        [Fact]
        public void SingleRound() {
            SingleInternal(new AlternatveMarkersRound(), "first $(two) third", "first second third");
        }

        [Fact]
        public void Single_End_Marker_Matches_Escaped_End_Marker() {
            SingleInternal(new AlternatveMarkersRound2(), "first $(two) third", "first second third");
        }

        [Fact]
        public void SingleCurly() {
            SingleInternal(DollarCurlyTokenMarkers.Instance, "first ${two} third", "first second third");
        }

        private void SingleInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleInteger() {
            SingleIntegerInternal(CurlyTokenMarkers.Instance, "first {two:D4} third", "first 0005 third");
        }

        [Fact]
        public void SingleIntegerRound() {
            SingleIntegerInternal(new AlternatveMarkersRound(), "first $(two:D4) third", "first 0005 third");
        }

        [Fact]
        public void SingleIntegerCurly() {
            SingleIntegerInternal(DollarCurlyTokenMarkers.Instance, "first ${two:D4} third", "first 0005 third");
        }

        private void SingleIntegerInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothing() {
            OpenEscapedCharacterYieldsNothingInternal(CurlyTokenMarkers.Instance, "first {{ third", "first { third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothingRound() {
            OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersRound(), "first $(( third", "first $( third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothingCurly() {
            OpenEscapedCharacterYieldsNothingInternal(DollarCurlyTokenMarkers.Instance, "first ${{ third", "first ${ third");
        }

        private void OpenEscapedCharacterYieldsNothingInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacement() {
            OpenEscapedCharacterYieldsReplacementInternal(CurlyTokenMarkers.Instance, "first {{{two} third", "first {second third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacementRound() {
            OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersRound(), "first $(($(two) third", "first $(second third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacementCurly() {
            OpenEscapedCharacterYieldsReplacementInternal(DollarCurlyTokenMarkers.Instance, "first ${{${two} third", "first ${second third");
        }

        private void OpenEscapedCharacterYieldsReplacementInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MissingTokenValue() {
            MissingTokenValueInternal(CurlyTokenMarkers.Instance, "first {missing} third", "first {missing} third");
        }

        [Fact]
        public void MissingTokenValueRound() {
            MissingTokenValueInternal(new AlternatveMarkersRound(), "first $(missing) third", "first $(missing) third");
        }

        [Fact]
        public void MissingTokenValueCurly() {
            MissingTokenValueInternal(DollarCurlyTokenMarkers.Instance, "first ${missing} third", "first ${missing} third");
        }

        private void MissingTokenValueInternal(ITokenMarkers markers, string original, string expected) {
            var Matcher = new RegexTokenParser(markers);

            var tokenValues = new Dictionary<string, object> { { "$(two)", "second" } };

            string actual = original.FormatDictionary(tokenValues, parser: Matcher);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Custom_Matcher_Resolves_Tokens_And_Returns_Mapped_String() {
            string expected = "first second third";
            string input = "first $(two) third";
            var mockTokenMatcher = new Mock<ITokenParser>();
            mockTokenMatcher.Setup(x => x.Parse(input)).Returns(new SegmentedString(new ISegment[]
            {
                new StringSegment("first "),
                new TokenSegment("$(two)", "two", null, null),
                new StringSegment(" third"),
            }));
            mockTokenMatcher.SetupGet(x => x.TokenNameComparer).Returns(StringComparer.CurrentCultureIgnoreCase);
            mockTokenMatcher.Setup(x => x.RemoveTokenMarkers("$(two)")).Returns("two");

            var actual = input.FormatToken("$(two)", "second", parser: mockTokenMatcher.Object);

            Assert.Equal(expected, actual);
        }
    }
}
