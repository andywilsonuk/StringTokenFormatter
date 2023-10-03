using Moq;
using StringTokenFormatter.Impl;
using Xunit;

namespace StringTokenFormatter.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void FormatFromObject_ObjectWithDefaultSettings_ReturnsExpandedString()
    {
        string pattern = "first {two} third";
        var valuesObject = new { two = 2 };

        string actual = pattern.FormatFromObject(valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_ObjectWithSettings_ReturnsExpandedString()
    {
        string pattern = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
            NameComparer = StringComparer.OrdinalIgnoreCase,
        };
        var valuesObject = new { Two = 2 };

        string actual = pattern.FormatFromObject(valuesObject, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_SingleValueWithDefaultSettings_ReturnsExpandedString()
    {
        string pattern = "first {two} third";

        string actual = pattern.FormatFromSingle("two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_SingleValueWithSettings_ReturnsExpandedString()
    {
        string pattern = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        string actual = pattern.FormatFromSingle("two", 2, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_FuncWithDefaultSettings_ReturnsExpandedString()
    {
        string pattern = "first {two} third";

        string actual = pattern.FormatFromFunc((string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_FuncWithSettings_ReturnsExpandedString()
    {
        string pattern = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        string actual = pattern.FormatFromFunc((string _token) => 2, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_PairsWithDefaultSettings_ReturnsExpandedString()
    {
        string pattern = "first {two} third";
        var tokenValues = new Dictionary<string, object> { { "two", 2 } };

        string actual = pattern.FormatFromPairs(tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_PairsWithSettings_ReturnsExpandedString()
    {
        string pattern = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var tokenValues = new Dictionary<string, object> { { "two", "second" } };

        string actual = pattern.FormatFromPairs(tokenValues, settings);

        string expected = "first second third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_ContainerWithDefaultSettings_ReturnsExpandedString()
    {
        string pattern = "first {two} third";
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        string actual = pattern.FormatFromContainer(valuesStub.Object);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_ContainerWithSettings_ReturnsExpandedString()
    {
        string pattern = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
            NameComparer = StringComparer.OrdinalIgnoreCase,
        };
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        string actual = pattern.FormatFromContainer(valuesStub.Object, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
}
