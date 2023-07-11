using Xunit;

namespace StringTokenFormatter.Tests;
public class CompositeTokenValueContainerTests
{
    private const string expected1 = "replaced1";
    private const string expected2 = "replaced2";
    private readonly ITokenValueContainer container1 = TokenValueContainer.FromValue("token1", expected1);
    private readonly ITokenValueContainer container2 = TokenValueContainer.FromValue("token2", expected2);
    private readonly ITokenValueContainer compositeContainer;

    public CompositeTokenValueContainerTests()
    {
        compositeContainer = TokenValueContainer.Combine(container1, container2);
    }

    [Fact]
    public void Cascade_To_First_Container_For_Mapping()
    {
        string input = "{token1}";
        var actual = input.FormatContainer(compositeContainer);

        Assert.Equal(expected1, actual);
    }

    [Fact]
    public void Cascade_To_Second_Container_For_Mapping()
    {
        string input = "{token2}";
        var actual = input.FormatContainer(compositeContainer);

        Assert.Equal(expected2, actual);
    }

    [Fact]
    public void Cascade_No_Map()
    {
        string input = "{token3}";
        var actual = input.FormatContainer(compositeContainer);

        Assert.Equal(input, actual);
    }
}
