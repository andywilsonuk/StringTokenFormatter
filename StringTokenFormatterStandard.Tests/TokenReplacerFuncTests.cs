using System;
using Xunit;
using System.Collections.Generic;
using StringTokenFormatter;
using System.Globalization;

namespace StringTokenFormatterStandard.Tests
{
    public class TokenReplacerFuncTests
    {
        [Fact]
        public void CallbackFunctionForValue()
        {
            string pattern = "first {two} third";
            Func<string, object> func = (token) => { return "second"; };
            var tokenValues = new Dictionary<string, object> { { "two", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CallbackFunctionForValueMixedCase()
        {
            string pattern = "first {Two} third";
            Func<string, object> func = (token) => { return "second"; };
            var tokenValues = new Dictionary<string, object> { { "two", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CallbackFunctionForValueUnused()
        {
            string pattern = "first {two} third";
            bool notCalled = true;
            Func<string, object> func = (token) => { notCalled = false;  return "second"; };
            var tokenValues = new Dictionary<string, object> { { "notmine", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first {two} third";
            Assert.Equal(expected, actual);
            Assert.True(notCalled);
        }

        [Fact]
        public void CallbackFunctionForStringValue()
        {
            string pattern = "first {two} third";
            Func<string, string> func = (token) => { return "second"; };
            var tokenValues = new Dictionary<string, object> { { "two", func } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CallbackLazyFunctionForValue()
        {
            string pattern = "first {two} third";
            Lazy<object> lazy = new Lazy<object>(() => { return "second"; });
            var tokenValues = new Dictionary<string, object> { { "two", lazy } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CallbackLazyFunctionForStringValue()
        {
            string pattern = "first {two} third";
            Lazy<string> lazy = new Lazy<string>(() => { return "second"; });
            var tokenValues = new Dictionary<string, object> { { "two", lazy } };

            string actual = new TokenReplacer().Format(null, pattern, tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }
    }
}