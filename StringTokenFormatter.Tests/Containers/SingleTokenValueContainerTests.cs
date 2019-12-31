using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests.Containers {
    public class SingleTokenValueContainerTests {

        [Fact]
        public void Formatting_Single_Value_Returns_In_Mapped_String() {
            string pattern = "first {two} third";

            var actual = pattern.FormatToken("two", "second");

            Assert.Equal("first second third", actual);
        }

        [Fact]
        public void Formatting_Single_Value_With_Markers_Returns_In_Mapped_String() {
            string pattern = "first {two} third";

            var actual = pattern.FormatToken("{two}", "second");

            Assert.Equal("first second third", actual);
        }

        [Fact]
        public void Formatting_Incorrect_Single_Value_Returns_In_Original_String() {
            string pattern = "first {two} third";

            var actual = pattern.FormatToken("notmine", "second");

            Assert.Equal("first {two} third", actual);
        }

        [Fact]
        public void Formatting_Single_Value_With_Custom_Mapper_Returns_In_Mapped_String() {
            string pattern = "first {two} third";
            object customMapping = "custom";
            var mockMapper = new Mock<ITokenValueConverter>();
            mockMapper.Setup(x => x.TryConvert(It.Is<IMatchedToken>(y => y.Token == "two"), "second", out customMapping))
                .Returns(true);
            var mappers = new List<ITokenValueConverter> { mockMapper.Object };

            var actual = pattern.FormatToken("two", "second", TokenValueFormatter.Default, mockMapper.Object, TokenParser.Default);

            Assert.Equal("first custom third", actual);
        }

        [Fact]
        public void Formatting_Single_Value_With_Custom_Formatter_Returns_In_Mapped_String() {
            string pattern = "first {two} third";
            var mockFormatter = new Mock<ITokenValueFormatter>();
            mockFormatter.Setup(x => x.Format(It.Is<TokenSegment>(y => y.Token == "two"), "second", "", ""))
                .Returns("custom");

            var actual = pattern.FormatToken("two", "second", mockFormatter.Object, TokenValueConverter.Default, TokenParser.Default);

            Assert.Equal("first custom third", actual);
        }

    }
}
