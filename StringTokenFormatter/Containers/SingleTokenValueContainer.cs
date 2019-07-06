using System;

namespace StringTokenFormatter {

    public class SingleTokenValueContainer<T> : ITokenValueContainer {
        private readonly string token;
        private readonly T value;
        private readonly ITokenParser matcher;

        public SingleTokenValueContainer(string tokenMarker, T mapValue, ITokenParser Parser = default) {
            if (string.IsNullOrEmpty(tokenMarker)) throw new ArgumentNullException(nameof(tokenMarker));

            Parser = Parser ?? TokenParser.Default;

            token = Parser.RemoveTokenMarkers(tokenMarker);
            value = mapValue;
            matcher = Parser ?? throw new ArgumentNullException(nameof(Parser));
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped) {
            if (matcher.TokenNameComparer.Equals(token, matchedToken.Token)) {
                mapped = value;
                return true;
            }
            mapped = null;
            return false;
        }
    }

}
