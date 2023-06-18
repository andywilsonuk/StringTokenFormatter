namespace StringTokenFormatter.Impl.TokenValueContainers;

/// <summary>
/// Prevents replacement of null token values.
/// </summary>
internal sealed class IgnoreNullTokenValueContainerImpl : ITokenValueContainer {
    private readonly ITokenValueContainer child;
    public IgnoreNullTokenValueContainerImpl(ITokenValueContainer child) {
        child = child ?? throw new ArgumentNullException(nameof(child));
        
        this.child = child;
    }

    public TryGetResult TryMap(ITokenMatch matchedToken) {
        var ret = child.TryMap(matchedToken);

        if (ret.IsSuccess && ret.Value is null) {
            ret = default;
        }
        return ret;
    }
}
