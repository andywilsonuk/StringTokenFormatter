using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StringTokenFormatter;
using System.Globalization;

namespace StringTokenFormatter.Tests
{
    [TestClass]
    public class TokenReplacerFuncTests
    {
        [TestMethod]
        public void CallbackFunctionForValue()
        {
            string pattern = "first {two} third";
            Func<string, object> func = (token) => { return "second"; };
            var tokenValues = new Dictionary<string, object> { { "two", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CallbackFunctionForValueMixedCase()
        {
            string pattern = "first {Two} third";
            Func<string, object> func = (token) => { return "second"; };
            var tokenValues = new Dictionary<string, object> { { "two", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CallbackFunctionForValueUnused()
        {
            string pattern = "first {two} third";
            bool notCalled = true;
            Func<string, object> func = (token) => { notCalled = false;  return "second"; };
            var tokenValues = new Dictionary<string, object> { { "notmine", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {two} third";
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(notCalled);
        }

        [TestMethod]
        public void CallbackFunctionForStringValue()
        {
            string pattern = "first {two} third";
            Func<string, string> func = (token) => { return "second"; };
            var tokenValues = new Dictionary<string, object> { { "two", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CallbackLazyFunctionForValue()
        {
            string pattern = "first {two} third";
            Lazy<object> lazy = new Lazy<object>(() => { return "second"; });
            var tokenValues = new Dictionary<string, object> { { "two", lazy } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CallbackLazyFunctionForStringValue()
        {
            string pattern = "first {two} third";
            Lazy<string> lazy = new Lazy<string>(() => { return "second"; });
            var tokenValues = new Dictionary<string, object> { { "two", lazy } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.AreEqual(expected, actual);
        }
    }
}
