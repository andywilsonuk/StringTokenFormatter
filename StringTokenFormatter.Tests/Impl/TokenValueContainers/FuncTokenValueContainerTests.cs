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
        var container = TokenValueContainerFactory.FromFunc(settings, func);

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
        var container = TokenValueContainerFactory.FromFunc(settings, func);

        var actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = null }, actual);
    }
}