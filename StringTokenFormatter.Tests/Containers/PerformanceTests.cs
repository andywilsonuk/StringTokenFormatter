using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests
{
    public class PerformanceTests
    {
        [Fact]
        public void CompareCompiled()
        {
            var Iterations = 100_000_0;

            var Format = "{Name} is {Age}";
            var ParsedFormat = SegmentedString.Create(Format);

            var Variables = new PerformanceTokenTest() {
                Name = "John Smith",
                Age = 21,
            };

            var Replacer = new TokenReplacer();

            var SW1 = System.Diagnostics.Stopwatch.StartNew();
            var Container1 = new ObjectPropertiesTokenValueContainer<PerformanceTokenTest>(Variables, TokenReplacer.DefaultMatcher);
            for (int i = 0; i < Iterations; i++) {
                var Output1 = Replacer.FormatFromContainer(ParsedFormat, Container1);
            }
            SW1.Stop();

            var SW2 = System.Diagnostics.Stopwatch.StartNew();
            var Container2 = new ObjectPropertiesTokenValueContainer<PerformanceTokenTest>(Variables, TokenReplacer.DefaultMatcher);
            for (int i = 0; i < Iterations; i++) {
                var Output2 = Replacer.FormatFromContainer(ParsedFormat, Container2);
            }
            SW2.Stop();




        }
    }

    public class PerformanceTokenTest {
        public string Name { get; set; }
        public int Age { get; set; }

        public static bool IsStatic { get; set; }
        private static bool IsPrivate { get; set; }

        public string GetterOnly => "Get Only";
        public string SetterOnly { set { } }

    }

}
