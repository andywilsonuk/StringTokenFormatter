using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests
{
    public class CascadingTokenValueContainerTests
    {
        private const string expected1 = "replaced1";
        private const string expected2 = "replaced2";
        private readonly ITokenValueContainer container1 = new SingleTokenValueContainer("token1", expected1, TokenReplacer.DefaultMatcher);
        private readonly ITokenValueContainer container2 = new SingleTokenValueContainer("token2", expected2, TokenReplacer.DefaultMatcher);
        private readonly ITokenValueContainer cascadingContainer;
        private readonly TokenReplacer tokenReplacer = new TokenReplacer();

        public CascadingTokenValueContainerTests()
        {
            cascadingContainer = new CascadingTokenValueContainer(new[] { container1, container2 });
        }

        [Fact]
        public void Cascade_To_First_Container_For_Mapping()
        {
            string input = "{token1}";
            var actual = tokenReplacer.FormatFromContainer(input, cascadingContainer);

            Assert.Equal(expected1, actual);
        }

        [Fact]
        public void Cascade_To_Second_Container_For_Mapping()
        {
            string input = "{token2}";
            var actual = tokenReplacer.FormatFromContainer(input, cascadingContainer);

            Assert.Equal(expected2, actual);
        }

        [Fact]
        public void Cascade_No_Map()
        {
            string input = "{token3}";
            var actual = tokenReplacer.FormatFromContainer(input, cascadingContainer);

            Assert.Equal(input, actual);
        }
    }
}
