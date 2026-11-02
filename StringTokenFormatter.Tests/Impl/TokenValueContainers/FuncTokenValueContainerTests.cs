namespace StringTokenFormatter.Tests;

public class FuncTokenValueContainerTests
{
    [Fact]
    public void TryMap_FuncReturnsObject_ReturnsSuccess()
    {
        static int? func(string token) => 1;
        StringTokenFormatterSettings settings = new()
        {
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        FuncTokenValueContainer<int?> container = TokenValueContainerFactory.FromFunc(settings, func);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = 1 }, actual);
    }

    [Fact]
    public void TryMap_FuncReturnsNull_ReturnsSuccess()
    {
        static int? func(string token) => null;
        StringTokenFormatterSettings settings = new()
        {
            TokenResolutionPolicy = TokenResolutionPolicy.ResolveAll,
        };
        FuncTokenValueContainer<int?> container = TokenValueContainerFactory.FromFunc(settings, func);

        TryGetResult actual = container.TryMap("a");

        Assert.Equal(new TryGetResult { IsSuccess = true, Value = null }, actual);
    }
}