using Xunit;
using System.Collections.Generic;
using Moq;
using System;

namespace StringTokenFormatter.Tests
{
    public class AlternatveTokenMarkerTests
    {
        [Fact]
        public void Single()
        {
            SingleInternal(new DefaultTokenMarkers(), "first {two} third", "first second third");
        }

        [Fact]
        public void SingleRound()
        {
            SingleInternal(new AlternatveMarkersRound(), "first $(two) third", "first second third");
        }

        [Fact]
        public void SingleCurly()
        {
            SingleInternal(new AlternatveMarkersCurly(), "first ${two} third", "first second third");
        }

        private void SingleInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer(markers).FormatFromDictionary(original, tokenValues);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleInteger()
        {
            SingleIntegerInternal(new DefaultTokenMarkers(), "first {two:D4} third", "first 0005 third");
        }

        [Fact]
        public void SingleIntegerRound()
        {
            SingleIntegerInternal(new AlternatveMarkersRound(), "first $(two:D4) third", "first 0005 third");
        }

        [Fact]
        public void SingleIntegerCurly()
        {
            SingleIntegerInternal(new AlternatveMarkersCurly(), "first ${two:D4} third", "first 0005 third");
        }

        private void SingleIntegerInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = new TokenReplacer(markers).FormatFromDictionary(original, tokenValues);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothing()
        {
            OpenEscapedCharacterYieldsNothingInternal(new DefaultTokenMarkers(), "first {{ third", "first { third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothingRound()
        {
            OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersRound(), "first $(( third", "first $( third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothingCurly()
        {
            OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersCurly(), "first ${{ third", "first ${ third");
        }

        private void OpenEscapedCharacterYieldsNothingInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer(markers).FormatFromDictionary(original, tokenValues);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacement()
        {
            OpenEscapedCharacterYieldsReplacementInternal(new DefaultTokenMarkers(), "first {{{two} third", "first {second third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacementRound()
        {
            OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersRound(), "first $(($(two) third", "first $(second third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacementCurly()
        {
            OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersCurly(), "first ${{${two} third", "first ${second third");
        }

        private void OpenEscapedCharacterYieldsReplacementInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer(markers).FormatFromDictionary(original, tokenValues);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MissingTokenValue()
        {
            MissingTokenValueInternal(new DefaultTokenMarkers(), "first {missing} third", "first {missing} third");
        }

        [Fact]
        public void MissingTokenValueRound()
        {
            MissingTokenValueInternal(new AlternatveMarkersRound(), "first $(missing) third", "first $(missing) third");
        }

        [Fact]
        public void MissingTokenValueCurly()
        {
            MissingTokenValueInternal(new AlternatveMarkersCurly(), "first ${missing} third", "first ${missing} third");
        }

        private void MissingTokenValueInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "$(two)", "second" } };

            string actual = new TokenReplacer(markers).FormatFromDictionary(original, tokenValues);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Custom_Matcher_Resolves_Tokens_And_Returns_Mapped_String()
        {
            string expected = "first second third";
            string input = "first $(two) third";
            var mockTokenMatcher = new Mock<ITokenMatcher>();
            mockTokenMatcher.Setup(x => x.SplitSegments(input)).Returns(new IMatchingSegment[]
            {
                new TextMatchingSegment("first "),
                new TokenMatchingSegment("$(two)", "two", null, null),
                new TextMatchingSegment(" third"),
            });
            mockTokenMatcher.SetupGet(x => x.TokenNameComparer).Returns(StringComparer.CurrentCultureIgnoreCase);
            mockTokenMatcher.Setup(x => x.RemoveTokenMarkers("$(two)")).Returns("two");

            string actual = new TokenReplacer(mockTokenMatcher.Object, TokenReplacer.DefaultMappers, TokenReplacer.DefaultFormatter).FormatFromSingle(input, "$(two)", "second");

            Assert.Equal(expected, actual);
        }
    }
}
