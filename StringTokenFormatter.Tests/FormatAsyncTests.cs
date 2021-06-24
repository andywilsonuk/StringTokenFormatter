using System;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StringTokenFormatter.Tests
{
    public class FormatAsyncTests
    {
        class Container : ITokenValueContainerAsync
        {
            public Task<bool> TryMapAsync(IMatchedToken matchedToken, out object? mapped)
            {
                if (matchedToken.Token == "token") {
                    mapped = "mapped";
                    return Task.FromResult(true);
                }
                mapped = null;
                return Task.FromResult(false);
            }
        }

        [Fact]
        public async Task Formatting_Single_Value_Using_A_Function_Returns_In_Mapped_String()
        {
            var container = new Container();
            var segments = new SegmentedString(new ISegment[] { new StringSegment("text"), new TokenSegment("{token}", "token", default, default) });

            var actual = await segments.FormatAsync(container, default, default);

            Assert.Equal("textmapped", actual);
        }
    }
}
