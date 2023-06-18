namespace StringTokenFormatter.Impl.TokenValueContainers; 
internal class EmptyTokenValueContainerImpl : ITokenValueContainer {

    public static EmptyTokenValueContainerImpl Instance { get; } = new EmptyTokenValueContainerImpl();

    private EmptyTokenValueContainerImpl() {

    }

    public TryGetResult TryMap(ITokenMatch matchedToken) {
        var ret = default(TryGetResult);

        return ret;
    }
}
