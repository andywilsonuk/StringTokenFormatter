namespace StringTokenFormatter.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void FormatFromSingle_WithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";

        string actual = source.FormatFromSingle("two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_WithSettings_ReturnsExpandedString()
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
    public void FormatFromPairs_WithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var tokenValues = new Dictionary<string, object> { { "two", 2 } };

        string actual = source.FormatFromPairs(tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_WithSettings_ReturnsExpandedString()
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
    public void FormatFromTuples_WithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var tokenValues = new [] { ("two", 2) };

        string actual = source.FormatFromTuples(tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromTuples_WithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var tokenValues = new [] { ("two", 2) };

        string actual = source.FormatFromTuples(tokenValues, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_WithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var valuesObject = new { Two = 2 };

        string actual = source.FormatFromObject(valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_WithSettings_ReturnsExpandedString()
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
    public void FormatFromFunc_WithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";

        string actual = source.FormatFromFunc((string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_WithSettings_ReturnsExpandedString()
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
    public void FormatFromContainer_WithDefaultSettings_ReturnsExpandedString()
    {
        string source = "first {two} third";
        var valuesContainer = new BasicContainer().Add("two", 2);

        string actual = source.FormatFromContainer(valuesContainer);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_WithSettings_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var valuesContainer = new BasicContainer().Add("two", 2);

        string actual = source.FormatFromContainer(valuesContainer, settings);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
}