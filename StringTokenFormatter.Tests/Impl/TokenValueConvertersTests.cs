namespace StringTokenFormatter.Tests;

public class TokenValueConvertersTests
{
    [Fact]
    public void NullConverter_IsNull_ReturnsSuccess()
    {
        object? source = null;
        var actual = TokenValueConverterFactory.NullConverter()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source }, actual);
    }

    [Fact]
    public void NullConverter_IsNotNull_ReturnsDefault()
    {
        object? source = "a";
        var actual = TokenValueConverterFactory.NullConverter()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void PrimitiveConverter_IsString_ReturnsSuccess()
    {
        object? source = "a";
        var actual = TokenValueConverterFactory.PrimitiveConverter()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source }, actual);
    }

    [Fact]
    public void PrimitiveConverter_IsDecimal_ReturnsSuccess()
    {
        object? source = 1;
        var actual = TokenValueConverterFactory.PrimitiveConverter()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source }, actual);
    }

    [Fact]
    public void PrimitiveConverter_IsObject_ReturnsDefault()
    {
        object? source = new {};
        var actual = TokenValueConverterFactory.PrimitiveConverter()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void LazyConverter_IsLazy_ReturnsSuccess()
    {
        object? source = new Lazy<string>(() => "a");
        var actual = TokenValueConverterFactory.LazyConverter<string>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "a" }, actual);
    }

    [Fact]
    public void LazyConverter_IsNotLazy_ReturnsDefault()
    {
        object? source = string.Empty;
        var actual = TokenValueConverterFactory.LazyConverter<string>()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void FuncConverter_IsFunc_ReturnsSuccess()
    {
        object? source = () => "a";
        var actual = TokenValueConverterFactory.FuncConverter<string>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "a" }, actual);
    }

    [Fact]
    public void FuncConverter_IsNotFunc_ReturnsDefault()
    {
        object? source = string.Empty;
        var actual = TokenValueConverterFactory.FuncConverter<string>()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void TokenFuncConverter_IsFunc_ReturnsSuccess()
    {
        object? source = (string _token) => "a";
        var actual = TokenValueConverterFactory.TokenFuncConverter<string>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "a" }, actual);
    }

    [Fact]
    public void TokenFuncConverter_IsNotFunc_ReturnsDefault()
    {
        object? source = string.Empty;
        var actual = TokenValueConverterFactory.TokenFuncConverter<string>()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void ToStringConverter_IsDecimal_ReturnsSuccess()
    {
        int? source = 1;
        var actual = TokenValueConverterFactory.ToStringConverter<int>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source.ToString() }, actual);
    }

     [Fact]
    public void ToStringConverter_IsWrongType_ReturnsDefault()
    {
        int? source = null;
        var actual = TokenValueConverterFactory.ToStringConverter<decimal>()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void ToStringConverter_IsNull_ReturnsDefault()
    {
        object? source = null;
        var actual = TokenValueConverterFactory.ToStringConverter<int>()(source, string.Empty);

        Assert.Equal(default, actual);
    }
}