namespace StringTokenFormatter.Tests;

public class BasicContainer : ITokenValueContainer
{
    private readonly Dictionary<string, object> tokens = new();

    public BasicContainer Add(string tokenName, object value)
    {
        tokens[tokenName] = value;
        return this;
    }

    public TryGetResult TryMap(string token) => tokens.TryGetValue(token, out object? value) ? TryGetResult.Success(value) : default;
}