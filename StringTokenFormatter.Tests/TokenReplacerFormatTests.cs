using Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;

namespace StringTokenFormatter.Tests {
    public class TokenReplacerFormatTests {

        [Fact]
        public void Custom_Container_Maps_String_Input_To_Values() {
            var container = new Mock<ITokenValueContainer>();
            object value = "second";
            container.Setup(x => x.TryMap(It.Is<IMatchedToken>(y => y.Token == "two"), out value)).Returns(true);
            string pattern = "first {two}";

            string actual = pattern.FormatContainer(container.Object);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Custom_Container_Maps_Segmented_String_Input_To_Values() {
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
