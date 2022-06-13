using System;
using Xunit;

namespace StringTokenFormatter.Tests {
    public class TokenReplacerFuncTests
    {


        [Fact]
        public void Formatting_Single_Value_Using_A_Function_Returns_In_Mapped_String()
        {
            string pattern = "first {two} third";
            Func<string> func = () => { return "second"; };

            var actual = pattern.FormatToken("two", func);

            Assert.Equal("first second third", actual);
        }
    }
}
