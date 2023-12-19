namespace StringTokenFormatter.Impl;

public sealed class SingleTokenValueContainer<T> : ITokenValueContainer where T : notnull
{
    private readonly string tokenName;
    private readonly T value;
    private readonly ITokenValueContainerSettings settings;

    internal SingleTokenValueContainer(ITokenValueContainerSettings settings, string tokenName, T value)
    {
        this.settings = ValidateArgs.AssertNotNull(settings, nameof(settings));
        if (string.IsNullOrEmpty(tokenName)) { throw new TokenContainerException("Empty string cannot be used as token name"); }
        this.tokenName = tokenName;
        this.value = value;
    }

    public TryGetResult TryMap(string token) =>
        settings.NameComparer.Equals(token, tokenName) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
