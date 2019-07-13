using Xunit;
using System.Collections.Generic;
using System.Globalization;

namespace StringTokenFormatter.Tests
{
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
            string pattern = "first {two} third";

            string actual = pattern.FormatToken("two", "second", formatter: TokenValueFormatter.From(CultureInfo.CurrentCulture));

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DictionaryValueThroughExtensionWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatDictionary(tokenValues, formatter: TokenValueFormatter.From(CultureInfo.CurrentCulture));

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

        [Fact]
        public void Interface_Excludes_Class_Members()
        {
            var Test = new TestClass() {
                First= 1,
                Second = "Second"
            };

            var pattern = "{First} {Second}";
            var actual = pattern.FormatToken<ITestInterface>(Test);
            var expected = "1 {Second}";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Derived_Includes_Base_Members()
        {
            var Test = new DerivedClass() {
                First = 1,
                Second = "Second",
                Third = "Third"
                
            };

            var pattern = "{First} {Second} {Third}";
            var actual = pattern.FormatToken(Test);
            var expected = "1 Second Third";

            Assert.Equal(expected, actual);
        }


        private interface ITestInterface
        {
            int First { get; set; }
        }

        public class TestClass : ITestInterface
        {
            public int First { get; set; }
            public string Second { get; set; }
        }

        public class DerivedClass : TestClass
        {
            public string Third { get; set; }
        }


    }
}
