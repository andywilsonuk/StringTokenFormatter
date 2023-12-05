namespace StringTokenFormatter.Tests;

public class TokenSyntaxExtensionsTests
{
    [Fact]
    public void Tokenize_SimpleTokenName_ReturnsWrappedToken()
    {
        string tokenName = "abc";
        var syntax = CommonTokenSyntax.Curly;
        
        string actual = syntax.Tokenize(tokenName);

        Assert.Equal("{abc}", actual);
    }
}