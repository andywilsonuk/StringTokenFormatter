using Xunit;

namespace StringTokenFormatter.Tests;
public class CurlyTokenMarkerTests : TokenMarkerTestsBase
{

    static IInterpolationSettings Settings = (InterpolationSettings.DefaultBuilder with
    {
        TokenSyntax = CommonTokenSyntax.Curly
    }).Build();

    [Fact]
    public void Single()
    {
        SingleInternal(Settings, "first {two} third", "first second third");
    }


    [Fact]
    public void SingleInteger()
    {
        SingleIntegerInternal(Settings, "first {two:D4} third", "first 0005 third");
    }

    [Fact]
    public void OpenEscapedCharacterYieldsNothing()
    {
        OpenEscapedCharacterYieldsNothingInternal(Settings, "first {{ third", "first { third");
    }

    [Fact]
    public void OpenEscapedCharacterYieldsReplacement()
    {
        OpenEscapedCharacterYieldsReplacementInternal(Settings, "first {{{two} third", "first {second third");
    }

    [Fact]
    public void MissingTokenValue()
    {
        MissingTokenValueInternal(Settings, "first {missing} third", "first {missing} third");
    }

}
