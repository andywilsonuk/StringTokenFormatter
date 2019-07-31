using Xunit;

namespace StringTokenFormatter.Tests {
    public class DollarRoundTokenMarkerTests : TokenMarkerTestsBase {


        [Fact]
        public void SingleRound() {
            SingleInternal(DollarRoundTokenMarkers.Instance, "first $(two) third", "first second third");
        }

        [Fact]
        public void SingleIntegerRound() {
            SingleIntegerInternal(DollarRoundTokenMarkers.Instance, "first $(two:D4) third", "first 0005 third");
        }



        [Fact]
        public void OpenEscapedCharacterYieldsNothingRound() {
            OpenEscapedCharacterYieldsNothingInternal(DollarRoundTokenMarkers.Instance, "first $(( third", "first $( third");
        }




        [Fact]
        public void OpenEscapedCharacterYieldsReplacementRound() {
            OpenEscapedCharacterYieldsReplacementInternal(DollarRoundTokenMarkers.Instance, "first $(($(two) third", "first $(second third");
        }



        [Fact]
        public void MissingTokenValueRound() {
            MissingTokenValueInternal(DollarRoundTokenMarkers.Instance, "first $(missing) third", "first $(missing) third");
        }


    }
}
