using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StringTokenFormatter.Tests
{
    [TestClass]
    public class StringFormatTests
    {
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void InvalidStringFormatThrowsFormatException()
        {
            string pattern = "{{0}";

            string.Format(pattern, "a");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void InvalidStringFormatThrowsFormatException2()
        {
            string pattern = "{0";

            string.Format(pattern, "a");
        }

        [TestMethod]
        public void PaddedZeroInteger()
        {
            string pattern = "{0:D4}";

            string actual = string.Format(pattern, 5);

            string expected = "0005";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PaddedAlignmentInteger()
        {
            string pattern = "{0,10:D}";

            string actual = string.Format(pattern, -27);

            string expected = "       -27";
            Assert.AreEqual(expected, actual);
        }
    }
}
