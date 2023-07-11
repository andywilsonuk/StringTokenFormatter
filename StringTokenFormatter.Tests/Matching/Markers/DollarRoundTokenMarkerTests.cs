using Xunit;

namespace StringTokenFormatter.Tests;
public class DollarRoundTokenMarkerTests : TokenMarkerTestsBase
{

    static IInterpolationSettings Settings = (InterpolationSettings.DefaultBuilder with
    {
        TokenSyntax = CommonTokenSyntax.DollarRound
    }).Build();

    [Fact]
    public void SingleRound()
    {
        SingleInternal(Settings, "first $(two) third", "first second third");
    }

    [Fact]
    public void SingleIntegerRound()
    {
        SingleIntegerInternal(Settings, "first $(two:D4) third", "first 0005 third");
    }



    [Fact]
    public void OpenEscapedCharacterYieldsNothingRound()
    {
        OpenEscapedCharacterYieldsNothingInternal(Settings, "first $(( third", "first $( third");
    }




    [Fact]
    public void OpenEscapedCharacterYieldsReplacementRound()
    {
        OpenEscapedCharacterYieldsReplacementInternal(Settings, "first $(($(two) third", "first $(second third");
    }



    [Fact]
    public void MissingTokenValueRound()
    {
        MissingTokenValueInternal(Settings, "first $(missing) third", "first $(missing) third");
    }


}
