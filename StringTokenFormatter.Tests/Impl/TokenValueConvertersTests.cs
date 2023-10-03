namespace StringTokenFormatter.Tests;

public class TokenValueConvertersTests
{
    [Fact]
    public void FromNull_IsNull_ReturnsSuccess()
    {
        object? source = null;
        var actual = TokenValueConverters.FromNull()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source }, actual);
    }

    [Fact]
    public void FromNull_IsNotNull_ReturnsDefault()
    {
        object? source = "a";
        var actual = TokenValueConverters.FromNull()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void FromPrimitives_IsString_ReturnsSuccess()
    {
        object? source = "a";
        var actual = TokenValueConverters.FromPrimitives()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source }, actual);
    }

    [Fact]
    public void FromPrimitives_IsDecimal_ReturnsSuccess()
    {
        object? source = 1;
        var actual = TokenValueConverters.FromPrimitives()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = source }, actual);
    }

    [Fact]
    public void FromPrimitives_IsObject_ReturnsDefault()
    {
        object? source = new {};
        var actual = TokenValueConverters.FromPrimitives()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void FromLazy_IsLazy_ReturnsSuccess()
    {
        object? source = new Lazy<string>(() => "a");
        var actual = TokenValueConverters.FromLazy<string>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "a" }, actual);
    }

    [Fact]
    public void FromLazy_IsNotLazy_ReturnsDefault()
    {
        object? source = string.Empty;
        var actual = TokenValueConverters.FromLazy<string>()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void FromFunc_IsFunc_ReturnsSuccess()
    {
        object? source = () => "a";
        var actual = TokenValueConverters.FromFunc<string>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "a" }, actual);
    }

    [Fact]
    public void FromFunc_IsNotFunc_ReturnsDefault()
    {
        object? source = string.Empty;
        var actual = TokenValueConverters.FromFunc<string>()(source, string.Empty);

        Assert.Equal(default, actual);
    }

    [Fact]
    public void FromTokenFunc_IsFunc_ReturnsSuccess()
    {
        object? source = (string _token) => "a";
        var actual = TokenValueConverters.FromTokenFunc<string>()(source, string.Empty);

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = "a" }, actual);
    }

    [Fact]
    public void FromTokenFunc_IsNotFunc_ReturnsDefault()
    {
        object? source = string.Empty;
        var actual = TokenValueConverters.FromTokenFunc<string>()(source, string.Empty);

        Assert.Equal(default, actual);
    }
}