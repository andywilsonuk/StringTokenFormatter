using System;

namespace StringTokenFormatter {

    public class SingleTokenValueContainer<T> : ITokenValueContainer {
        private readonly string token;
        private readonly T value;
        private readonly ITokenParser matcher;

        public SingleTokenValueContainer(string tokenMarker, T mapValue, ITokenParser parser = default) {
            if (string.IsNullOrEmpty(tokenMarker)) throw new ArgumentNullException(nameof(tokenMarker));

            parser = parser ?? TokenParser.Default;

            token = parser.RemoveTokenMarkers(tokenMarker);
            value = mapValue;
            matcher = parser ?? throw new ArgumentNullException(nameof(parser));
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
