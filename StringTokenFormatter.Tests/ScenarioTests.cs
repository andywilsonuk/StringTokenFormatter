namespace StringTokenFormatter.Tests;

public class ScenarioTests
{
    private readonly InterpolatedStringResolver resolver = new(StringTokenFormatterSettings.Default);
    private readonly BasicContainer valuesContainer = new();

    [Fact]
    public void LoopAndConditional_StringWithLiteralValue3TimesWithoutSuppressedValue()
    {
        string source = "{:loop:3}lit{:if,IsValid}suppressed{:ifend}{:loopend}";
        valuesContainer.Add("IsValid", false);

        string actual = resolver.FromContainer(source, valuesContainer);

        Assert.Equal("litlitlit", actual);
    }

    [Fact]
    public void ConditionalPreventsInnerTokenMatching_StringWithoutSuppressedValue()
    {
        string source = "{:if,IsValid}{Suppressed}{:ifend}";
        valuesContainer.Add("IsValid", false);
        valuesContainer.Add("Suppressed", () => Assert.Fail("This value should not be called"));

        string actual = resolver.FromContainer(source, valuesContainer);

        Assert.Equal(string.Empty, actual);
    }
}