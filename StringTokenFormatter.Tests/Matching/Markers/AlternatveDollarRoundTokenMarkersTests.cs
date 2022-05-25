using Xunit;

namespace StringTokenFormatter.Tests {
    public class AlternatveDollarRoundTokenMarkersTests : TokenMarkerTestsBase {
        [Fact]
        public void Single_End_Marker_Matches_Escaped_End_Marker() {
            var Settings = new InterpolationSettingsBuilder
            {
                TokenSyntax = TokenSyntaxes.DollarRoundAlternative
            }.Build();

            SingleInternal(Settings, "first $(two) third", "first second third");
        }
    }
}
