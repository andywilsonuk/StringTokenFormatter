using Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;

namespace StringTokenFormatter.Tests
{
    public class TokenReplacerFormatTests
    {
        [Fact]
        public void EmptyStringValue()
        {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = string.Empty;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NotUsed()
        {
            string pattern = "first second";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MixedCase()
        {
            string pattern = "first {Two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Duplicate()
        {
            string pattern = "first {two} third {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MixedIn()
        {
            string pattern = "first{two}third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "firstsecondthird";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiple()
        {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" }, { "four", "fourth" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third fourth";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleReversed()
        {
            string pattern = "first {two} third {four}";
            var tokenValues = new Dictionary<string, object> { { "four", "fourth" }, { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third fourth";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NormalisedTokenKey()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "{two}", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleIntegerWithPadding()
        {
            string pattern = "first{two,10:D4} third";
            var tokenValues = new Dictionary<string, object> { { "two", 5 } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first      0005 third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CloseEscapeCharacterYieldsReplacement()
        {
            string pattern = "first {two}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second} third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Close_Escape_Character_Same_As_End_Character_Yields_Replacement_And_Escape_Marker()
        {
            var Matcher = new RegexTokenParser(new AlternatveMarkersRound2());

            string pattern = "first $(two)) third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues, Parser: Matcher);

            string expected = "first second) third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BothEscapeCharacterYieldsReplacement()
        {
            string pattern = "first {{{two}} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first {second} third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InvalidStringFormatIsHandled()
        {
            string pattern = "{{two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "{two}";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObject()
        {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = pattern.FormatToken(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObjectCaseMatch()
        {
            string pattern = "first {two} third";
            var tokenValues = new { Two = "second" };

            string actual = pattern.FormatToken(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObjectWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = pattern.FormatToken(tokenValues, Formatter: TokenValueFormatter.From(CultureInfo.CurrentCulture));

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleDictionaryWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues, Formatter: TokenValueFormatter.From(CultureInfo.CurrentCulture));

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EscapeAsLastCharacter()
        {
            string pattern = "first {{";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first {";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NonTokenisedOpenBracketsAreIgnored()
        {
            string pattern = "first { {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first { second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void String_Dictionary_With_Matching_Token_Mapped_Successfully()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, string> { { "{two}", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void String_Dictionary_With_Empty_Pattern_Returns_Empty_String()
        {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, string> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = string.Empty;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Null_String_Dictionary_Throws()
        {
            string pattern = "first {two} third";

            Assert.Throws<ArgumentNullException>(() => pattern.FormatDictionary((IDictionary<string, string>)null));
        }

        [Fact]
        public void Null_Object_Dictionary_Throws()
        {
            string pattern = "first {two} third";

            Assert.Throws<ArgumentNullException>(() => pattern.FormatDictionary((IDictionary<string, object>)null));
        }

        [Fact]
        public void Formatting_Single_Value_Returns_In_Mapped_String()
        {
            string pattern = "first {two} third";

            var actual = pattern.FormatToken("two", "second");

            Assert.Equal("first second third", actual);
        }

        [Fact]
        public void Formatting_Single_Value_With_Markers_Returns_In_Mapped_String()
        {
            string pattern = "first {two} third";

            var actual = pattern.FormatToken("{two}", "second");

            Assert.Equal("first second third", actual);
        }

        [Fact]
        public void Formatting_Incorrect_Single_Value_Returns_In_Original_String()
        {
            string pattern = "first {two} third";

            var actual = pattern.FormatToken("notmine", "second");

            Assert.Equal("first {two} third", actual);
        }

        [Fact]
        public void Formatting_Single_Value_With_Custom_Mapper_Returns_In_Mapped_String()
        {
            string pattern = "first {two} third";
            object customMapping = "custom";
            var mockMapper = new Mock<ITokenValueConverter>();
            mockMapper.Setup(x => x.TryConvert(It.Is<IMatchedToken>(y => y.Token == "two"), "second", out customMapping))
                .Returns(true);
            var mappers = new List<ITokenValueConverter> { mockMapper.Object };

            var actual = pattern.FormatToken("two", "second", TokenValueFormatter.Default, mockMapper.Object, TokenParser.Default);

            Assert.Equal("first custom third", actual);
        }

        [Fact]
        public void Formatting_Single_Value_With_Custom_Formatter_Returns_In_Mapped_String()
        {
            string pattern = "first {two} third";
            var mockFormatter = new Mock<ITokenValueFormatter>();
            mockFormatter.Setup(x => x.Format(It.Is<TokenSegment>(y => y.Token == "two"), "second"))
                .Returns("custom");

            var actual = pattern.FormatToken("two", "second", mockFormatter.Object, TokenValueConverter.Default, TokenParser.Default);

            Assert.Equal("first custom third", actual);
        }

        [Fact]
        public void Multiple_Line_Text_Replaces_All_Occurrences()
        {
            string pattern = "first {two}" + Environment.NewLine + "third {two}";
            var tokenValues = new Dictionary<string, object> { { "two", "sec" + Environment.NewLine + "ond" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first sec" + Environment.NewLine + "ond" + Environment.NewLine + "third sec" + Environment.NewLine + "ond";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Space_Within_Token_Matches()
        {
            string pattern = "first {The Token} third";
            var tokenValues = new Dictionary<string, object> { { "The Token", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Custom_Container_Maps_String_Input_To_Values()
        {
            var container = new Mock<ITokenValueContainer>();
            object value = "second";
            container.Setup(x => x.TryMap(It.Is<IMatchedToken>(y => y.Token == "two"), out value)).Returns(true);
            string pattern = "first {two}";

            string actual = pattern.FormatContainer(container.Object);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Custom_Container_Maps_Segmented_String_Input_To_Values()
        {
            var container = new Mock<ITokenValueContainer>();
            object value = "second";
            container.Setup(x => x.TryMap(It.Is<IMatchedToken>(y => y.Token == "two"), out value)).Returns(true);
            SegmentedString segments = new SegmentedString(new ISegment[]
            {
                new StringSegment("first "),
                new TokenSegment("{two}", "two", null, null),
            });

            string actual = segments.Format(container.Object);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }
    }
}
