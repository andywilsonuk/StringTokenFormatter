using StringTokenFormatter.Impl;

namespace StringTokenFormatter.Impl.TokenValueContainers {
    public class EmptyTokenValueContainer : ITokenValueContainer {

        public static EmptyTokenValueContainer Instance { get; } = new EmptyTokenValueContainer();

        private EmptyTokenValueContainer() {

        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = default(TryGetResult);

            return ret;
        }
    }
}
