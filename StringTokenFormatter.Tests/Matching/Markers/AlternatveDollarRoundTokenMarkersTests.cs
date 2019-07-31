using Xunit;

namespace StringTokenFormatter.Tests {
    public class AlternatveDollarRoundTokenMarkersTests : TokenMarkerTestsBase {
        [Fact]
        public void Single_End_Marker_Matches_Escaped_End_Marker() {
            SingleInternal(AlternatveDollarRoundTokenMarkers.Instance, "first $(two) third", "first second third");
        }
    }
}
