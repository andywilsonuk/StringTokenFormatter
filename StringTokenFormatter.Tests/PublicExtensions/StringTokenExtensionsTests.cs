using Xunit;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests {
    public class StringTokenExtensionsTests
    {
        [Fact]
        public void SingleValueThroughExtension()
        {
            string pattern = "first {two} third";

            string actual = pattern.FormatToken("two", "second");

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DictionaryValueThroughExtension()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleValueThroughExtensionWithCulture()
        {
            var Settings = new InterpolationSettingsBuilder
            {
                TokenValueFormatter = TokenValueFormatters.CurrentCulture,
            }.Build();

            string pattern = "first {two} third";

            string actual = pattern.FormatToken("two", "second", Settings);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DictionaryValueThroughExtensionWithCulture()
        {
            var Settings = new InterpolationSettingsBuilder
            {
                TokenValueFormatter = TokenValueFormatters.CurrentCulture,
            }.Build();

            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues, Settings);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringDictionariesAreNotHandledAsObject()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, string> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        


    }
}
