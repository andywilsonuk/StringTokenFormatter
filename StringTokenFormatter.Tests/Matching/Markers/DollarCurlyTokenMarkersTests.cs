using Xunit;

namespace StringTokenFormatter.Tests; 
public class DollarCurlyTokenMarkersTests : TokenMarkerTestsBase {
    
    static IInterpolationSettings Settings = (InterpolationSettings.DefaultBuilder with { 
        TokenSyntax = TokenSyntaxes.DollarCurly
    }).Build();
    
    [Fact]
    public void SingleCurly() {
        SingleInternal(Settings, "first ${two} third", "first second third");
    }

    [Fact]
    public void SingleIntegerCurly() {
        SingleIntegerInternal(Settings, "first ${two:D4} third", "first 0005 third");
    }

    [Fact]
    public void OpenEscapedCharacterYieldsNothingCurly() {
        OpenEscapedCharacterYieldsNothingInternal(Settings, "first ${{ third", "first ${ third");
    }

    [Fact]
    public void OpenEscapedCharacterYieldsReplacementCurly() {
        OpenEscapedCharacterYieldsReplacementInternal(Settings, "first ${{${two} third", "first ${second third");
    }

    [Fact]
    public void MissingTokenValueCurly() {
        MissingTokenValueInternal(Settings, "first ${missing} third", "first ${missing} third");
    }

}
