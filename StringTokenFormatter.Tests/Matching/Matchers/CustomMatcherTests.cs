using Xunit;
using Moq;
using System;

namespace StringTokenFormatter.Tests {

    public class CustomMatcherTests { 

        [Fact]
        public void Custom_Matcher_Resolves_Tokens_And_Returns_Mapped_String() {
            string expected = "first second third";
            string input = "first $(two) third";
            var mockTokenMatcher = new Mock<ITokenParser>();
            mockTokenMatcher.Setup(x => x.Parse(input)).Returns(new SegmentedString(new ISegment[]
            {
                new StringSegment("first "),
                new TokenSegment("$(two)", "two", null, null),
                new StringSegment(" third"),
            }));

            mockTokenMatcher.Setup(x => x.RemoveTokenMarkers("$(two)")).Returns("two");

            var actual = input.FormatToken("$(two)", "second", parser: mockTokenMatcher.Object);

            Assert.Equal(expected, actual);
        }
    }
}
