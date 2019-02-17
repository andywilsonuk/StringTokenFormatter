using Xunit;
using System.Collections.Generic;
using Moq;
using System;
using Xunit.Abstractions;

namespace StringTokenFormatter.Tests {
    public class PerformanceTests
    {

        private readonly ITestOutputHelper output;

        public PerformanceTests(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void MiscellaneousTest() {

            var OUTER = System.Diagnostics.Stopwatch.StartNew();
            var Args = new ExampleClass();
            var WarmUp = Format.FormatToken(Args);

            var INNER = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < COUNT; i++) {
                Format.FormatToken(Args);
            }
            INNER.Stop();

            OUTER.Stop();

            output.WriteLine($@"OUTER: {OUTER.Elapsed}");
            output.WriteLine($@"INNER: {OUTER.Elapsed}");
        }

        //These two methods allow me to see the performance comparison by comparing the elapsed time in the test explorer.
        [Fact]
        public void SegmentedStringFormat() {
            var Args = new ExampleClass();

            var Segment = SegmentedString.Create(Format);
            for (int i = 0; i < COUNT; i++) {
                Segment.FormatToken(Args);
            }
        }

        [Fact]
        public void StringFormat() {
            var Args = new ExampleClass();
            for (int i = 0; i < COUNT; i++) {
                Format.FormatToken(Args);
            }
        }

        const string Format = "{Property1} {Property3} {Property5} {Property7} {Property9}";
        const int COUNT = 100_000;


        public class ExampleClass {
            public string Property0 { get; set; } = "0";
            public int Property1 { get; set; } = 1;
            public long Property2 { get; set; } = 2;
            public DateTime Property3 { get; set; } = DateTime.Now;
            public string Property4 { get; set; } = "4";
            public int Property5 { get; set; } = 5;
            public long Property6 { get; set; } = 6;
            public DateTimeOffset Property7 { get; set; } = DateTimeOffset.UtcNow;
            public string Property8 { get; set; } = "8";
            public int Property9 { get; set; } = 9;

            public bool Property10 { get; set; } = true;

            public string SlowProperty {
                get {
                    System.Threading.Thread.Sleep(1);
                    return "Slow!";
                }
            }
        }

    }
}
