using Xunit;
using System.Collections.Generic;
using StringTokenFormatter;
using System.Globalization;

namespace StringTokenFormatterStandard.Tests
{
    public class TokenReplacerFormatTests
    {
        [Fact]
        public void EmptyStringValue()
        {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = string.Empty;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NotUsed()
        {
            string pattern = "first second";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MixedCase()
        {
            string pattern = "first {Two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Duplicate()
        {
            string pattern = "first {two} third {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MixedIn()
        {
            string pattern = "first{two}third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "firstsecondthird";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiple()
        {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" }, { "four", "fourth" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third fourth";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleReversed()
        {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "four", "fourth" }, { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third fourth";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NormalisedTokenKey()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "{two}", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleIntegerWithPadding()
        {
            string pattern = "first{two,10:D4} third";
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first      0005 third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CloseEscapeCharacterYieldsReplacement()
        {
            string pattern = "first {two}}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second} third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BothEscapeCharacterYieldsReplacement()
        {
            string pattern = "first {{{two}}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {second} third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InvalidStringFormatIsHandled()
        {
            string pattern = "{{nothing}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "{{nothing}";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObject()
        {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObjectCaseMatch()
        {
            string pattern = "first {two} third";
            var tokenValues = new { Two = "second" };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObjectWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = new TokenReplacer().Format(CultureInfo.CurrentCulture, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleDictionaryWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(CultureInfo.CurrentCulture, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EscapeAsLastCharacter()
        {
            string pattern = "first {{";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NonTokenisedOpenBracketsAreIgnored()
        {
            string pattern = "first { {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first { second";
            Assert.Equal(expected, actual);
        }
    }
}
