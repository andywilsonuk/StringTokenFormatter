namespace StringTokenFormatter.Tests;

public class TokenResolutionPolicyExtensionsTests
{
    [Theory]
    [InlineData(TokenResolutionPolicy.ResolveAll, null, true)]
    [InlineData(TokenResolutionPolicy.ResolveAll, "", true)]
    [InlineData(TokenResolutionPolicy.ResolveAll, "a", true)]
    [InlineData(TokenResolutionPolicy.ResolveAll, 1, true)]
    [InlineData(TokenResolutionPolicy.IgnoreNull, null, false)]
    [InlineData(TokenResolutionPolicy.IgnoreNull, "", true)]
    [InlineData(TokenResolutionPolicy.IgnoreNull, "a", true)]
    [InlineData(TokenResolutionPolicy.IgnoreNull, 1, true)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, null, false)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "", false)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, "a", true)]
    [InlineData(TokenResolutionPolicy.IgnoreNullOrEmpty, 1, true)]
    public void Satisfies_WithData_ReturnsExpected(TokenResolutionPolicy policy, object? source, bool expected)
    {
        bool actual = policy.Satisfies(source);

        Assert.Equal(expected, actual);
    }
}