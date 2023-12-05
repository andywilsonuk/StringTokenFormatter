namespace StringTokenFormatter.Tests;

public class InterpolatedStringResolverTests
{
    private readonly StringTokenFormatterSettings settings = new()
    {
        Syntax = CommonTokenSyntax.Round,
    };

    [Fact]
    public void FromSingle_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);

        string actual = resolver.FromSingle(source, "two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromSingle_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);

        string actual = resolver.FromSingle(interpolatedString, "two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromPairs_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var tokenValues = new Dictionary<string, object> { { "two", 2 } };

        string actual = resolver.FromPairs(source, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromPairs_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);
        var tokenValues = new Dictionary<string, object> { { "two", 2 } };

        string actual = resolver.FromPairs(interpolatedString, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void FromTuples_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var tokenValues = new[] { ("two", 2) };

        string actual = resolver.FromTuples(source, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromTuples_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);
        var tokenValues = new[] { ("two", 2) };

        string actual = resolver.FromTuples(interpolatedString, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void FromObject_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var valuesObject = new { Two = 2 };

        string actual = resolver.FromObject(source, valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromObject_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);
        var valuesObject = new { Two = 2 };

        string actual = resolver.FromObject(interpolatedString, valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromFunc_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);

        string actual = resolver.FromFunc(source, (string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromFunc_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);

        string actual = resolver.FromFunc(interpolatedString, (string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromContainer_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var valuesContainer = new BasicContainer().Add("two", 2);

        string actual = resolver.FromContainer(source, valuesContainer);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromContainer_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        var resolver = new InterpolatedStringResolver(settings);
        var interpolatedString = InterpolatedStringParser.Parse(source, settings);
        var valuesContainer = new BasicContainer().Add("two", 2);

        string actual = resolver.FromContainer(interpolatedString, valuesContainer);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
}