namespace StringTokenFormatter.Tests;

public class FuncTokenValueContainerTests
{
    [Fact]
    public void TryMap_FuncReturnsObject_ReturnsSuccess()
    {
        static int? func(string token) => 1;
        var settings = new StringTokenFormatterSettings
        {
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new FuncTokenValueContainer<int?>(func, settings);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void TryMap_FuncReturnsNull_ReturnsSuccess()
    {
        static int? func(string token) => null;
        var settings = new StringTokenFormatterSettings
        {
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        var container = new FuncTokenValueContainer<int?>(func, settings);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = null }, actual);
    }

    [Fact]
    public void TryMap_FuncReturnsNullPolicyViolation_ReturnsDefault()
    {
        static int? func(string token) => null;
        var settings = new StringTokenFormatterSettings
        {
            TokenResolutionPolicy = TokenResolutionPolicy.IgnoreNull,
        };
        var container = new FuncTokenValueContainer<int?>(func, settings);

        var actual = container.TryMap("a");

        Assert.Equal(default, actual);
    }
}