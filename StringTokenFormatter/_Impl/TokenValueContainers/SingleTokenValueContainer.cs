namespace StringTokenFormatter.Impl.TokenValueContainers
{

    internal class SingleTokenValueContainerImpl<T> : ITokenValueContainer {
        protected readonly string token;
        protected readonly T value;
        protected readonly ITokenNameComparer nameComparer;

        public SingleTokenValueContainerImpl(string token, T mapValue, ITokenNameComparer nameComparer) {

            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            this.token = token;

            this.nameComparer = nameComparer;
            value = mapValue;
        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = default(TryGetResult);

            if (nameComparer.Equals(token, matchedToken.Token)) {
                ret = TryGetResult.Success(value);
            }

            return ret;
        }
    }

}
