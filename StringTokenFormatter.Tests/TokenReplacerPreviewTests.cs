using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StringTokenFormatter;
using System.Globalization;

namespace StringTokenFormatter.Tests
{
    [TestClass]
    public class TokenReplacerPreviewTests
    {
        [TestMethod]
        public void EmptyStringValue()
        {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().FormatPreview(null, pattern, tokenValues);

            string expected = string.Empty;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FormatPreview()
        {
            string pattern = "first {two,10:D4} third {fourth}";
            var tokenValues = new Dictionary<string, object> { { "two", 2 }, { "fourth", 4} };

            string actual = new TokenReplacer().FormatPreview(null, pattern, tokenValues);

            string expected = "first {0,10:D4} third {1}";
            Assert.AreEqual(expected, actual);
        }
    }
}
