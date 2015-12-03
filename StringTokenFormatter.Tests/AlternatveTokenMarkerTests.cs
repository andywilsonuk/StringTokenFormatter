using System;
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
            this.SingleInternal(new DefaultTokenMarkers(), "first {two} third", "first second third");
        }

        [TestMethod]
        public void SingleRound()
        {
            this.SingleInternal(new AlternatveMarkersRound(), "first $(two) third", "first second third");
        }

        [TestMethod]
        public void SingleCurly()
        {
            this.SingleInternal(new AlternatveMarkersCurly(), "first ${two} third", "first second third");
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
            this.SingleIntegerInternal(new DefaultTokenMarkers(), "first {two:D4} third", "first 0005 third");
        }

        [TestMethod]
        public void SingleIntegerRound()
        {
            this.SingleIntegerInternal(new AlternatveMarkersRound(), "first $(two:D4) third", "first 0005 third");
        }

        [TestMethod]
        public void SingleIntegerCurly()
        {
            this.SingleIntegerInternal(new AlternatveMarkersCurly(), "first ${two:D4} third", "first 0005 third");
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
            this.OpenEscapedCharacterYieldsNothingInternal(new DefaultTokenMarkers(), "first {{ third", "first { third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsNothingRound()
        {
            this.OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersRound(), "first $(( third", "first $( third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsNothingCurly()
        {
            this.OpenEscapedCharacterYieldsNothingInternal(new AlternatveMarkersCurly(), "first ${{ third", "first ${ third");
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
            this.OpenEscapedCharacterYieldsReplacementInternal(new DefaultTokenMarkers(), "first {{{two} third", "first {second third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsReplacementRound()
        {
            this.OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersRound(), "first $(($(two) third", "first $(second third");
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsReplacementCurly()
        {
            this.OpenEscapedCharacterYieldsReplacementInternal(new AlternatveMarkersCurly(), "first ${{${two} third", "first ${second third");
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
            this.MissingTokenValueInternal(new DefaultTokenMarkers(), "first {missing} third", "first {missing} third");
        }

        [TestMethod]
        public void MissingTokenValueRound()
        {
            this.MissingTokenValueInternal(new AlternatveMarkersRound(), "first $(missing) third", "first $(missing) third");
        }

        [TestMethod]
        public void MissingTokenValueCurly()
        {
            this.MissingTokenValueInternal(new AlternatveMarkersCurly(), "first ${missing} third", "first ${missing} third");
        }

        private void MissingTokenValueInternal(TokenMarkers markers, string original, string expected)
        {
            var tokenValues = new Dictionary<string, object> { { "$(two)", "second" } };

            string actual = new TokenReplacer(markers).Format(null, original, tokenValues);

            Assert.AreEqual(expected, actual);
        }
    }
}
