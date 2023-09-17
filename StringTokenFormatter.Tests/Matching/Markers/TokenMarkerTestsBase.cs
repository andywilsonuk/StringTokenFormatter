using Xunit;

namespace StringTokenFormatter.Tests;

public abstract class TokenMarkerTestsBase
{
    protected static void SingleInternal(IInterpolationSettings settings, string original, string expected)
    {

        var tokenValues = new Dictionary<string, object> { { "two", "second" } };

        var actual = original.FormatDictionary(tokenValues, settings);

        Assert.Equal(expected, actual);
    }

    protected static void SingleIntegerInternal(IInterpolationSettings settings, string original, string expected)
    {

        var tokenValues = new Dictionary<string, object> { { "two", 5 } };

        var actual = original.FormatDictionary(tokenValues, settings);

        Assert.Equal(expected, actual);
    }

    protected static void OpenEscapedCharacterYieldsReplacementInternal(IInterpolationSettings settings, string original, string expected)
    {

        var tokenValues = new Dictionary<string, object> { { "two", "second" } };

        var actual = original.FormatDictionary(tokenValues, settings);

        Assert.Equal(expected, actual);
    }

    protected static void MissingTokenValueInternal(IInterpolationSettings settings, string original, string expected)
    {

        var tokenValues = new Dictionary<string, object> { { "$(two)", "second" } };

        var actual = original.FormatDictionary(tokenValues, settings);

        Assert.Equal(expected, actual);
    }

    protected static void OpenEscapedCharacterYieldsNothingInternal(IInterpolationSettings settings, string original, string expected)
    {

        var tokenValues = new Dictionary<string, object> { { "two", "second" } };

        var actual = original.FormatDictionary(tokenValues, settings);

        Assert.Equal(expected, actual);
    }


}
