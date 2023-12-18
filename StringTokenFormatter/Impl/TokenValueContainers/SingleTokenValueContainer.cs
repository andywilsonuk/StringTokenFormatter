namespace StringTokenFormatter.Impl;

/// <summary>
/// This Value Container matches a single token name.
/// </summary>
public class SingleTokenValueContainer<T> : ITokenValueContainer where T : notnull
{
    private readonly string tokenName;
    private readonly T value;
    private readonly ITokenValueContainerSettings settings;

    public SingleTokenValueContainer(string tokenName, T value, ITokenValueContainerSettings settings)
    {
        this.tokenName = tokenName;
        this.value = value;
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token) =>
        settings.NameComparer.Equals(token, tokenName) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
