using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

namespace StringTokenFormatter.Tests
{
    [TestClass]
    public class StringTokenExtensionsTests
    {
        [TestMethod]
        public void SingleValueThroughExtension()
        {
            string pattern = "first {two} third";

            string actual = pattern.FormatToken("two", "second");

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DictionaryValueThroughExtension()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatToken(tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleValueThroughExtensionWithCulture()
        {
            string pattern = "first {two} third";

            string actual = pattern.FormatToken(CultureInfo.CurrentCulture, "two", "second");

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DictionaryValueThroughExtensionWithCulture()
        {
            string pattern = "first {two} third";
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = pattern.FormatToken(CultureInfo.CurrentCulture, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }
    }
}
