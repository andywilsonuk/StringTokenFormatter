using AndyWilsonUk.StringTokenFormatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests
{
	[TestClass]
    public class AlternatveTokenMarkerTests
    {
        [TestMethod]
        public void Single()
        {
            SingleInternal(new DefaultTokenMarkers(), "first {two} third", "first second third");
        }

        [TestMethod]
        public void SingleRound()
        {
            SingleInternal(new AlternatveMarkersRound(), "first $(two) third", "first second third");
        }

        [TestMethod]
        public void SingleCurly()
        {
            SingleInternal(new AlternatveMarkersCurly(), "first ${two} third", "first second third");
        }

        private void SingleInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer(markers).Format(null, original, tokenValues);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleInteger()
        {
            SingleIntegerInternal(new DefaultTokenMarkers(), "first {two:D4} third", "first 0005 third");
        }

        [TestMethod]
        public void SingleIntegerRound()
        {
            SingleIntegerInternal(new AlternatveMarkersRound(), "first $(two:D4) third", "first 0005 third");
        }

        [TestMethod]
        public void SingleIntegerCurly()
        {
            SingleIntegerInternal(new AlternatveMarkersCurly(), "first ${two:D4} third", "first 0005 third");
        }

        private void SingleIntegerInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = new TokenReplacer(markers).Format(null, original, tokenValues);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsNothing()
        {
            OpenEscapedCharacterYieldsNothingInternal(new DefaultTokenMarkers(), "first {{ third", "first { third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsNothingRound()
        {
            OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersRound(), "first $(( third", "first $( third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsNothingCurly()
        {
            OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersCurly(), "first ${{ third", "first ${ third");
        }

        private void OpenEscapedCharacterYieldsNothingInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer(markers).Format(null, original, tokenValues);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsReplacement()
        {
            OpenEscapedCharacterYieldsReplacementInternal(new DefaultTokenMarkers(), "first {{{two} third", "first {second third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsReplacementRound()
        {
            OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersRound(), "first $(($(two) third", "first $(second third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsReplacementCurly()
        {
            OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersCurly(), "first ${{${two} third", "first ${second third");
        }

        private void OpenEscapedCharacterYieldsReplacementInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer(markers).Format(null, original, tokenValues);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MissingTokenValue()
        {
            MissingTokenValueInternal(new DefaultTokenMarkers(), "first {missing} third", "first {missing} third");
        }

        [TestMethod]
        public void MissingTokenValueRound()
        {
            MissingTokenValueInternal(new AlternatveMarkersRound(), "first $(missing) third", "first $(missing) third");
        }

        [TestMethod]
        public void MissingTokenValueCurly()
        {
            MissingTokenValueInternal(new AlternatveMarkersCurly(), "first ${missing} third", "first ${missing} third");
        }

        private void MissingTokenValueInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "$(two)", "second" } };

            string actual = new TokenReplacer(markers).Format(null, original, tokenValues);

            Assert.AreEqual(expected, actual);
        }
    }
}
