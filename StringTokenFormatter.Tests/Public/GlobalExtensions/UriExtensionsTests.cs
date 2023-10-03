namespace StringTokenFormatter.Tests;

public class UriExtensionsTests
{
    [Fact]
    public void FormatFromSingle_SingleValueWithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q={token}");

        var actual = source.FormatFromSingle("token", 2);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_SingleValueWithSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q=(token)");
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        var actual = source.FormatFromSingle("token", 2, settings);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_PairsWithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q={token}");
        var tokenValues = new Dictionary<string, object> { { "token", 2 } };

        var actual = source.FormatFromPairs(tokenValues);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromPairs_PairsWithSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q=(token)");
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var tokenValues = new Dictionary<string, object> { { "token", 2 } };

        var actual = source.FormatFromPairs(tokenValues, settings);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_FuncWithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q={token}");

        var actual = source.FormatFromFunc((string _token) => 2);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromObject_ObjectWithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q={token}");
        var valuesObject = new { Token = 2 };

        var actual = source.FormatFromObject(valuesObject);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void FormatFromObject_ObjectWithSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q=(token)");
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var valuesObject = new { Token = 2 };

        var actual = source.FormatFromObject(valuesObject, settings);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_ContainerWithDefaultSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q={token}");
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("token")).Returns(TryGetResult.Success(2));

        var actual = source.FormatFromContainer(valuesStub.Object);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromFunc_FuncWithSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q=(token)");
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };

        var actual = source.FormatFromFunc((string _token) => 2, settings);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromContainer_ContainerWithSettings_ReturnsExpandedAbsoluteUri()
    {
        var source = new Uri("http://locallhost/?q=(token)");
        var settings = new StringTokenFormatterSettings {
            Syntax = CommonTokenSyntax.Round,
        };
        var valuesStub = new Mock<ITokenValueContainer>();
        valuesStub.Setup(x => x.TryMap("token")).Returns(TryGetResult.Success(2));

        var actual = source.FormatFromContainer(valuesStub.Object, settings);

        var expected = new Uri("http://locallhost/?q=2");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatFromSingle_RelativeUri_ReturnsRelativeUri()
    {
        var source = new Uri("/?q={token}", UriKind.Relative);

        var actual = source.FormatFromSingle("token", 2);

        var expected = new Uri("/?q=2", UriKind.Relative);
        Assert.Equal(expected, actual);
    }
}