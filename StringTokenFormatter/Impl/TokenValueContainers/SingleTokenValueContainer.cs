using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public class SingleTokenValueContainer<T> : ITokenValueContainer
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
        token == tokenName && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
