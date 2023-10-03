namespace StringTokenFormatter.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void FormatFromSingle_SingleValueWithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";

        string actual = source.FormatFromSingle("two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_SingleValueWithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        string actual = source.FormatFromSingle("two", 2, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_PairsWithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var tokenValues = new Dictionary<string, object> { { "two", 2 } };

        string actual = source.FormatFromPairs(tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_PairsWithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var tokenValues = new Dictionary<string, object> { { "two", 2 } };

        string actual = source.FormatFromPairs(tokenValues, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_ObjectWithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var valuesObject = new { Two = 2 };

        string actual = source.FormatFromObject(valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_ObjectWithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var valuesObject = new { Two = 2 };

        string actual = source.FormatFromObject(valuesObject, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_FuncWithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";

        string actual = source.FormatFromFunc((string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_FuncWithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        string actual = source.FormatFromFunc((string _token) => 2, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_ContainerWithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        string actual = source.FormatFromContainer(valuesStub.Object);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_ContainerWithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("two")).Returns(TryGetResult.Success(2));

        string actual = source.FormatFromContainer(valuesStub.Object, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
}