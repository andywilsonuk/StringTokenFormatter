using Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;
using StringTokenFormatter.Impl;
using StringTokenFormatter.Impl.InterpolatedStrings;
using StringTokenFormatter.Impl.InterpolatedStringSegments;
using System.Collections.Immutable;

namespace StringTokenFormatter.Tests {
    public class TokenReplacerFormatTests {

        [Fact]
        public void Custom_Container_Maps_String_Input_To_Values() {
            var container = new Mock<ITokenValueContainer>();
            object? value = "second";
            container.Setup(x => x.TryMap(It.Is<ITokenMatch>(y => y.Token == "two"))).Returns(TryGetResult.Success(value));
            string pattern = "first {two}";

            string actual = pattern.FormatContainer(container.Object);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Custom_Container_Maps_Segmented_String_Input_To_Values() {
            var container = new Mock<ITokenValueContainer>();
            object value = "second";
            container.Setup(x => x.TryMap(It.Is<ITokenMatch>(y => y.Token == "two"))).Returns(TryGetResult.Success(value));
            InterpolatedString segments = new InterpolatedString(new IInterpolatedStringSegment[]
            {
                new LiteralInterpolatedStringSegment("first "),
                new TokenInterpolatedStringSegment("{two}", "two", null, null),
            }.ToImmutableArray());

            string actual = segments.FormatContainer(container.Object);

            string expected = "first second";
            Assert.Equal(expected, actual);
        }

    }
}
