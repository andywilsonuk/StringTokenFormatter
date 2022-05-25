using Xunit;
using StringTokenFormatter;

namespace NsTest {
    public class NsTestClass {
        [Fact]
        public void Large_Sample_1() {
            var Settings = new InterpolationSettingsBuilder() {
                TokenSyntax = TokenSyntaxes.DollarRound,
            }.Build();
        }
    }
}
