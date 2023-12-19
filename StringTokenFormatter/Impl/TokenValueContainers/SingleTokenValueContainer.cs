namespace StringTokenFormatter.Impl;

public sealed class SingleTokenValueContainer<T> : ITokenValueContainer where T : notnull
{
    private readonly string tokenName;
    private readonly T value;
    private readonly ITokenValueContainerSettings settings;

    internal SingleTokenValueContainer(ITokenValueContainerSettings settings, string tokenName, T value)
    {
        if (string.IsNullOrEmpty(tokenName)) { throw new InvalidTokenNameException("Empty string cannot be used as token name"); }
        this.tokenName = tokenName;
        this.value = value;
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token) =>
        settings.NameComparer.Equals(token, tokenName) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
