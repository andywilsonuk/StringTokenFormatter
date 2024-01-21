namespace StringTokenFormatter.Tests;

public class StringFormatParityTests
{
    private readonly InterpolatedStringResolver resolver = new(StringTokenFormatterSettings.Default);
    private readonly BasicContainer valuesContainer = new();

    [Theory]
    [InlineData("", 2)]
    [InlineData(":N", 2)]
    [InlineData(",6:P", 2)]
    [InlineData(",-6:P", 2)]
    [InlineData(",-2:D", 123456)]
    [InlineData(",2:D", 123456)]
    [InlineData(":N", -123456)]
    [InlineData(",-2:N", -123456)]
    [InlineData(",2:N", -123456)]
    public void IntPatternParsing_EqualsStringFormat(string pattern, int value)
    {
        string source = $"{{num{pattern}}}";
        valuesContainer.Add("num", value);
        string expected = string.Format($"{{0{pattern}}}", value);

        string actual = resolver.FromContainer(source, valuesContainer);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(":HH:mm")]
    public void DateTimePatternParsing_EqualsStringFormat(string pattern)
    {
        string source = $"{{num{pattern}}}";
        var value = new DateTime(2000, 1, 2, 3, 4, 5);
        valuesContainer.Add("num", value);
        string expected = string.Format($"{{0{pattern}}}", value);

        string actual = resolver.FromContainer(source, valuesContainer);

        Assert.Equal(expected, actual);
    }
}