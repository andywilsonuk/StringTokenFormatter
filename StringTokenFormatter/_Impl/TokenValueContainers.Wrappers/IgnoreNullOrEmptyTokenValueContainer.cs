namespace StringTokenFormatter.Impl.TokenValueContainers {
    /// <summary>
    /// Prevents replacement of null or empty token values.
    /// </summary>
    internal sealed class IgnoreNullOrEmptyTokenValueContainerImpl : ITokenValueContainer {
        private readonly ITokenValueContainer child;
        public IgnoreNullOrEmptyTokenValueContainerImpl(ITokenValueContainer child) {
            this.child = child;
        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = child.TryMap(matchedToken);

            if (ret.IsSuccess && ret.Value is null or string { Length: 0}) {
                ret = default;
            }
            return ret;
        }
    }

}
