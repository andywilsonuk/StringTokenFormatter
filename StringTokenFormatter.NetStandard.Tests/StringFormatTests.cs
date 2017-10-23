using System;
using Xunit;

namespace StringTokenFormatter.Tests
{

	public class StringFormatTests
    {
        [Fact]
        public void InvalidStringFormatThrowsFormatException()
        {
            string pattern = "{{0}";

			var ex = Assert.Throws<FormatException>(() => string.Format(pattern, "a"));

			Assert.Equal("Input string was not in a correct format.", ex.Message);
		}

        [Fact]
        //[ExpectedException(typeof(FormatException))]
        public void InvalidStringFormatThrowsFormatException2()
        {
            string pattern = "{0";

			var ex = Assert.Throws<FormatException>(() => string.Format(pattern, "a"));

			Assert.Equal("Input string was not in a correct format.", ex.Message);
		}

		[Fact]
        public void PaddedZeroInteger()
        {
            string pattern = "{0:D4}";

            string actual = string.Format(pattern, 5);

            string expected = "0005";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PaddedAlignmentInteger()
        {
            string pattern = "{0,10:D}";

            string actual = string.Format(pattern, -27);

            string expected = "       -27";
            Assert.Equal(expected, actual);
        }
    }
}
