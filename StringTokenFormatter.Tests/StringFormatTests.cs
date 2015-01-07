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
        public void PaddedInteger()
        {
            string pattern = "{0:D4}";

            string actual = string.Format(pattern, 5);

            string expected = "0005";
            Assert.AreEqual(expected, actual);
        }
    }
}
