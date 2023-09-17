namespace StringTokenFormatter.Impl;

public record TokenValue<T>(string Token, T Value);

public class TokenValueContainer<T> : ITokenValueContainer
{
    private readonly Dictionary<string, T> pairs;
    private readonly ITokenValueContainerSettings settings;

    public TokenValueContainer(IEnumerable<TokenValue<T>> source, ITokenValueContainerSettings settings)
    {
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        pairs = new Dictionary<string, T>(settings.NameComparer);
        foreach (var pair in source.Where(p => !string.IsNullOrEmpty(p.Token)))
        {
            pairs[pair.Token] = pair.Value;
        }
    }

    public TryGetResult TryMap(string token) =>
        pairs.TryGetValue(token, out var value) && settings.TokenResolutionPolicy.Satisfies(value) ? TryGetResult.Success(value) : default;
}
