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
        if (string.IsNullOrEmpty(tokenName)) { throw new InvalidTokenNameException("Empty string cannot be used as token name"); }
        this.tokenName = tokenName;
        this.value = value;
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public TryGetResult TryMap(string token) =>
        settings.NameComparer.Equals(token, tokenName) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;

    public static SingleTokenValueContainer<T2> From<T2>(string tokenName, T2 value, ITokenValueContainerSettings settings) where T2 : notnull => new(tokenName, value, settings);
}
