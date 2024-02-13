namespace StringTokenFormatter.Impl;

public sealed class SingleTokenValueContainer<T> : ITokenValueContainer where T : notnull
{
    private readonly ITokenValueContainerSettings settings;
    private readonly string tokenName;
    private readonly T value;

    internal SingleTokenValueContainer(ITokenValueContainerSettings settings, string tokenName, T value)
    {
        this.settings = Guard.NotNull(settings, nameof(settings)).Validate();
        this.tokenName = Guard.NotEmpty(tokenName, nameof(tokenName));
        this.value = value;
    }

    public TryGetResult TryMap(string token) => settings.NameComparer.Equals(token, tokenName) ? TryGetResult.Success(value) : default;
}
