using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests {
    public class DictionaryTokenValueContainerTests {

        [Fact]
        public void EmptyStringValue() {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = string.Empty;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NotUsed() {
            string pattern = "first second";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MixedCase() {
            string pattern = "first {Two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Duplicate() {
            string pattern = "first {two} third {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MixedIn() {
            string pattern = "first{two}third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "firstsecondthird";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiple() {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" }, { "four", "fourth" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third fourth";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleReversed() {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "four", "fourth" }, { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third fourth";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NormalisedTokenKey() {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "{two}", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleIntegerWithPadding() {
            string pattern = "first{two,10:D4} third";
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first      0005 third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CloseEscapeCharacterYieldsReplacement() {
            string pattern = "first {two}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second} third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Close_Escape_Character_Same_As_End_Character_Yields_Replacement_And_Escape_Marker() {
            var Matcher = new RegexTokenParser(AlternatveDollarRoundTokenMarkers.Instance);

            string pattern = "first $(two)) third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues, parser: Matcher);

            string expected = "first second) third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BothEscapeCharacterYieldsReplacement() {
            string pattern = "first {{{two}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first {second} third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InvalidStringFormatIsHandled() {
            string pattern = "{{two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "{two}";
            Assert.Equal(expected, actual);
        }



        [Fact]
        public void SingleDictionaryWithCulture() {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues, formatter: TokenValueFormatter.From(CultureInfo.CurrentCulture));

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EscapeAsLastCharacter() {
            string pattern = "first {{";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first {";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NonTokenisedOpenBracketsAreIgnored() {
            string pattern = "first { {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first { second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void String_Dictionary_With_Matching_Token_Mapped_Successfully() {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, string> { { "{two}", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void String_Dictionary_With_Empty_Pattern_Returns_Empty_String() {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, string> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = string.Empty;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Null_String_Dictionary_Throws() {
            string pattern = "first {two} third";

            Assert.Throws<ArgumentNullException>(() => pattern.FormatDictionary((IDictionary<string, string>)null));
        }

        [Fact]
        public void Null_Object_Dictionary_Throws() {
            string pattern = "first {two} third";

            Assert.Throws<ArgumentNullException>(() => pattern.FormatDictionary((IDictionary<string, object>)null));
        }

        [Fact]
        public void Multiple_Line_Text_Replaces_All_Occurrences() {
            string pattern = "first {two}" + Environment.NewLine + "third {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "sec" + Environment.NewLine + "ond" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first sec" + Environment.NewLine + "ond" + Environment.NewLine + "third sec" + Environment.NewLine + "ond";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Space_Within_Token_Matches() {
            string pattern = "first {The Token} third";
            var tokenValues = new Dictionary<string, object> { { "The Token", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

    }
}
