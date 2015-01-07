﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StringTokenFormatter;
using System.Globalization;

namespace StringTokenFormatter.Tests
{
    [TestClass]
    public class TokenReplacerTests
    {
        [TestMethod]
        public void Single()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NotUsed()
        {
            string pattern = "first second";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MixedCase()
        {
            string pattern = "first {Two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Duplicate()
        {
            string pattern = "first {two} third {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third second";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MixedIn()
        {
            string pattern = "first{two}third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "firstsecondthird";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Multiple()
        {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" }, { "four", "fourth" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third fourth";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MultipleReversed()
        {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "four", "fourth" }, { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third fourth";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalisedTokenKey()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "{two}", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MissingTokenValue()
        {
            string pattern = "first {missing} third";
            var tokenValues = new Dictionary<string, object> { { "{two}", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {missing} third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleInteger()
        {
            string pattern = "first {two:D4} third";
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first 0005 third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleIntegerWithPadding()
        {
            string pattern = "first{two,10:D4} third";
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first      0005 third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsNothing()
        {
            string pattern = "first {{ third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first { third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OpenEscapedCharacterYieldsReplacement()
        {
            string pattern = "first {{{two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CloseEscapeCharacterYieldsReplacement()
        {
            string pattern = "first {two}}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second} third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BothEscapeCharacterYieldsReplacement()
        {
            string pattern = "first {{{two}}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {second} third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void InvalidFormatThrowsFormatException()
        {
            string pattern = "{{nothing}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            new TokenReplacer().Format(null, pattern, tokenValues);
        }

        [TestMethod]
        public void AnonymousObject()
        {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AnonymousObjectCaseMatch()
        {
            string pattern = "first {two} third";
            var tokenValues = new { Two = "second" };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AnonymousObjectWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = new TokenReplacer().Format(CultureInfo.CurrentCulture, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleDictionaryWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(CultureInfo.CurrentCulture, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EscapeAsLastCharacter()
        {
            string pattern = "first {{";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {";
            Assert.AreEqual(expected, actual);
        }
    }
}
