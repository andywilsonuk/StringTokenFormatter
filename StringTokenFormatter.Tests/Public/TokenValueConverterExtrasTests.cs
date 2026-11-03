namespace StringTokenFormatter.Tests;

public class TokenValueConverterExtrasTests
{
    [Fact]
    public void ToStringConverter_UsesToStringForMatchingType()
    {
        object source = new ToStringValue();

        TryGetResult actual = TokenValueConverterExtras.ToStringConverter<ToStringValue>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "converted" }, actual);
    }

    private sealed class ToStringValue
    {
        public override string ToString() => "converted";
    }
}
