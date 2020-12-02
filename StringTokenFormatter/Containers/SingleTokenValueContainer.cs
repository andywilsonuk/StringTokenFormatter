using System;

namespace StringTokenFormatter {

    public class SingleTokenValueContainer<T> : ITokenValueContainer {
        protected readonly string token;
        protected readonly T value;
        protected readonly ITokenNameComparer nameComparer;

        public SingleTokenValueContainer(string token, T mapValue, ITokenNameComparer? nameComparer = default, ITokenParser? parser = default) {

            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            parser ??= TokenParser.Default;
            token = parser.RemoveTokenMarkers(token);

            this.token = token;

            this.nameComparer = nameComparer ?? TokenNameComparer.Default;
            value = mapValue;
        }

        public virtual bool TryMap(IMatchedToken matchedToken, out object? mapped) {
            if (nameComparer.Comparer.Equals(token, matchedToken.Token)) {
                mapped = value;
                return true;
            }
            mapped = null;
            return false;
        }
    }

}
