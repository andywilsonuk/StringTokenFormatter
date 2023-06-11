using Xunit;

namespace StringTokenFormatter.Tests
{

    public class RegexTokenParserTests {

        [Fact]
        public void When_Passed_A_String_Containing_Tokens_The_TokensMatched_Method_Returns_The_Matched_Tokens() {
            var parser = InterpolatedStringParsers.Default;
            var input = "{a}, {b,10:D}";
            var expected = new[] { "a", "b" };

            var actual = parser.Parse(input).Segments.OfType<IInterpolatedStringSegmentToken>().Select(x => x.Token);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void When_Passed_A_String_Not_Containing_Tokens_The_TokensMatched_Method_Returns_An_Empty_Enumerable() {
            var parser = InterpolatedStringParsers.Default;

            var input = "a, b";

            var actual = parser.Parse(input).Segments.OfType<IInterpolatedStringSegmentToken>().Select(x => x.Token);

            Assert.Empty(actual);
        }
    }

}
