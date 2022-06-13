using Xunit;

namespace StringTokenFormatter.Tests {
    public class PerformanceTests
    {
        [Fact]
        public void CompareCompiled()
        {
            var Iterations = 1_000_000;

            var Format = "{Name} is {Age}";
            var ParsedFormat = Format.ToInterpolatedString();

            var Variables = new PerformanceTokenTest() {
                Name = "John Smith",
                Age = 21,
            };


            var SW1 = System.Diagnostics.Stopwatch.StartNew();
            var Container1 = TokenValueContainer.FromObject(Variables);
            for (int i = 0; i < Iterations; i++) {
                var Output1 = ParsedFormat.FormatContainer(Container1);
            }
            SW1.Stop();

            var SW2 = System.Diagnostics.Stopwatch.StartNew();
            var Container2 = TokenValueContainer.FromObject(Variables);
            for (int i = 0; i < Iterations; i++) {
                var Output2 = ParsedFormat.FormatContainer(Container2);
            }
            SW2.Stop();

        }

        [Fact]
        public void CompareParsing() {
            var Iterations = 1_000_000;

            var Format = "{Name} is {Age}";
            
            for (int i = 0; i < Iterations; i++) {
                var ParsedFormat = Format.ToInterpolatedString();
            }
            

        }

    }

    public class PerformanceTokenTest {
        public string? Name { get; set; }
        public int Age { get; set; }

        public static bool IsStatic { get; set; }
        private static bool IsPrivate { get; set; }

        public string GetterOnly => "Get Only";
        public string SetterOnly { set { } }

    }

}
