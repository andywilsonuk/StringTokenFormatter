using Xunit;

namespace StringTokenFormatter.Tests {
    public class DollarCurlyTokenMarkersTests : TokenMarkerTestsBase {
        [Fact]
        public void SingleCurly() {
            SingleInternal(DollarCurlyTokenMarkers.Instance, "first ${two} third", "first second third");
        }

        [Fact]
        public void SingleIntegerCurly() {
            SingleIntegerInternal(DollarCurlyTokenMarkers.Instance, "first ${two:D4} third", "first 0005 third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothingCurly() {
            OpenEscapedCharacterYieldsNothingInternal(DollarCurlyTokenMarkers.Instance, "first ${{ third", "first ${ third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacementCurly() {
            OpenEscapedCharacterYieldsReplacementInternal(DollarCurlyTokenMarkers.Instance, "first ${{${two} third", "first ${second third");
        }

        [Fact]
        public void MissingTokenValueCurly() {
            MissingTokenValueInternal(DollarCurlyTokenMarkers.Instance, "first ${missing} third", "first ${missing} third");
        }

    }
}
