using AndyWilsonUk.StringTokenFormatter;
using System.Collections.Generic;
using Xunit;

namespace StringTokenFormatter.Tests
{

	public class TokenReplacerPreviewTests
    {
        [Fact]
        public void EmptyStringValue()
        {
            string pattern = string.Empty;
            var tokenValues = new Dictionary<string, object> { { "two", "second" } };

            string actual = new TokenReplacer().FormatPreview(null, pattern, tokenValues);

            string expected = string.Empty;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatPreview()
        {
            string pattern = "first {two,10:D4} third {fourth}";
            var tokenValues = new Dictionary<string, object> { { "two", 2 }, { "fourth", 4} };

            string actual = new TokenReplacer().FormatPreview(null, pattern, tokenValues);

            string expected = "first {0,10:D4} third {1}";
            Assert.Equal(expected, actual);
        }
    }
}
