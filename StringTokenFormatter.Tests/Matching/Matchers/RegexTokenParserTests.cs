using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests {

    public class RegexTokenParserTests {
        private readonly RegexTokenParser matcher = new RegexTokenParser();

        [Fact]
        public void When_Passed_A_String_Containing_Tokens_The_TokensMatched_Method_Returns_The_Matched_Tokens() {
            string input = "{a}, {b,10:D}";
            var expected = new[] { "a", "b" };

            var actual = matcher.Parse(input).Segments.OfType<TokenSegment>().Select(x => x.Token);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void When_Passed_A_String_Not_Containing_Tokens_The_TokensMatched_Method_Returns_An_Empty_Enumerable() {
            string input = "a, b";

            var actual = matcher.Parse(input).Segments.OfType<TokenSegment>().Select(x => x.Token);

            Assert.Empty(actual);
        }
    }

}
