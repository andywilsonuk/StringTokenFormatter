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
        InterpolatedStringResolver resolver = new(settings);

        string actual = resolver.FromSingle(source, "two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromSingle_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);

        string actual = resolver.FromSingle(interpolatedString, "two", 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromPairs_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        Dictionary<string, object> tokenValues = new() { { "two", 2 } };

        string actual = resolver.FromPairs(source, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromPairs_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);
        Dictionary<string, object> tokenValues = new() { { "two", 2 } };

        string actual = resolver.FromPairs(interpolatedString, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }


    [Fact]
    public void FromPairs_StringSourceWithParams_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        KeyValuePair<string, int> tokenValue1 = new("one", 1);
        KeyValuePair<string, int> tokenValue2 = new("two", 2);

        string actual = resolver.FromPairs(source, tokenValue1, tokenValue2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromPairs_InterpolatedStringSourceWithParams_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);
        KeyValuePair<string, int> tokenValue1 = new("one", 1);
        KeyValuePair<string, int> tokenValue2 = new("two", 2);

        string actual = resolver.FromPairs(interpolatedString, tokenValue1, tokenValue2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromTuples_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        (string, int)[] tokenValues = [("two", 2)];

        string actual = resolver.FromTuples(source, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromTuples_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);
        (string, int)[] tokenValues = [("two", 2)];

        string actual = resolver.FromTuples(interpolatedString, tokenValues);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromTuples_StringSourceWithParams_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);

        string actual = resolver.FromTuples<object>(source, ("one", 1), ("two", "2"));

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromTuples_InterpolatedStringSourceWithParams_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);

        string actual = resolver.FromTuples<object>(interpolatedString, ("one", 1), ("two", "2"));

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromObject_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        var valuesObject = new { Two = 2 };

        string actual = resolver.FromObject(source, valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromObject_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);
        var valuesObject = new { Two = 2 };

        string actual = resolver.FromObject(interpolatedString, valuesObject);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromFunc_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);

        string actual = resolver.FromFunc(source, (string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromFunc_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);

        string actual = resolver.FromFunc(interpolatedString, (string _token) => 2);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromContainer_StringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        BasicContainer valuesContainer = new BasicContainer().Add("two", 2);

        string actual = resolver.FromContainer(source, valuesContainer);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromContainer_InterpolatedStringSource_ReturnsExpandedString()
    {
        string source = "first (two) third";
        InterpolatedStringResolver resolver = new(settings);
        InterpolatedString interpolatedString = InterpolatedStringParser.Parse(source, settings);
        BasicContainer valuesContainer = new BasicContainer().Add("two", 2);

        string actual = resolver.FromContainer(interpolatedString, valuesContainer);

        string expected = "first 2 third";
        Assert.Equal(expected, actual);
    }
}