using Xunit;

namespace StringTokenFormatter.Tests {
    public class CurlyTokenMarkerTests : TokenMarkerTestsBase {



        [Fact]
        public void Single() {
            SingleInternal(CurlyTokenMarkers.Instance, "first {two} third", "first second third");
        }


        [Fact]
        public void SingleInteger() {
            SingleIntegerInternal(CurlyTokenMarkers.Instance, "first {two:D4} third", "first 0005 third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsNothing() {
            OpenEscapedCharacterYieldsNothingInternal(CurlyTokenMarkers.Instance, "first {{ third", "first { third");
        }

        [Fact]
        public void OpenEscapedCharacterYieldsReplacement() {
            OpenEscapedCharacterYieldsReplacementInternal(CurlyTokenMarkers.Instance, "first {{{two} third", "first {second third");
        }

        [Fact]
        public void MissingTokenValue() {
            MissingTokenValueInternal(CurlyTokenMarkers.Instance, "first {missing} third", "first {missing} third");
        }

    }
}
