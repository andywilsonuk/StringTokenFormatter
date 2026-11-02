namespace StringTokenFormatter.Tests;

public class UriExtensionsTests
{
    [Fact]
    public void FormatFromSingle_WithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q={token}");

        Uri actual = source.FormatFromSingle("token", 2);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_WithSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q=(token)");
        StringTokenFormatterSettings settings = new()
        {
            Syntax = CommonTokenSyntax.Round,
        };

        Uri actual = source.FormatFromSingle("token", 2, settings);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_WithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q={token}");
        Dictionary<string, object> tokenValues = new() { { "token", 2 } };

        Uri actual = source.FormatFromPairs(tokenValues);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_WithSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q=(token)");
        StringTokenFormatterSettings settings = new()
        {
            Syntax = CommonTokenSyntax.Round,
        };
        Dictionary<string, object> tokenValues = new() { { "token", 2 } };

        Uri actual = source.FormatFromPairs(tokenValues, settings);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromTuples_WithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q={token}");
        (string, int)[] tokenValues = [("token", 2)];

        Uri actual = source.FormatFromTuples(tokenValues);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromTuples_WithSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q=(token)");
        StringTokenFormatterSettings settings = new()
        {
            Syntax = CommonTokenSyntax.Round,
        };
        (string, int)[] tokenValues = [("token", 2)];

        Uri actual = source.FormatFromTuples(tokenValues, settings);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_WithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q={token}");

        Uri actual = source.FormatFromFunc((string _token) => 2);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_WithSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q=(token)");
        StringTokenFormatterSettings settings = new()
        {
            Syntax = CommonTokenSyntax.Round,
        };

        Uri actual = source.FormatFromFunc((string _token) => 2, settings);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_WithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q={token}");
        var valuesObject = new { Token = 2 };

        Uri actual = source.FormatFromObject(valuesObject);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_WithSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q=(token)");
        StringTokenFormatterSettings settings = new()
        {
            Syntax = CommonTokenSyntax.Round,
        };
        var valuesObject = new { Token = 2 };

        Uri actual = source.FormatFromObject(valuesObject, settings);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_WithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q={token}");
        BasicContainer valuesContainer = new BasicContainer().Add("token", 2);

        Uri actual = source.FormatFromContainer(valuesContainer);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_WithSettings_ReturnsExpandedAbsoluteUri()
    {
        Uri source = new("http://locallhost/?q=(token)");
        StringTokenFormatterSettings settings = new()
        {
            Syntax = CommonTokenSyntax.Round,
        };
        BasicContainer valuesContainer = new BasicContainer().Add("token", 2);

        Uri actual = source.FormatFromContainer(valuesContainer, settings);

        Uri expected = new("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_RelativeUri_ReturnsRelativeUri()
    {
        Uri source = new("/?q={token}", UriKind.Relative);

        Uri actual = source.FormatFromSingle("token", 2);

        Uri expected = new("/?q=2", UriKind.Relative);
        Assert.Equal(expected, actual);
    }
}