using System;

namespace StringTokenFormatter
{
    public class SingleTokenValueContainer : ITokenValueContainer
    {
        private readonly string token;
        private readonly object value;
        private readonly ITokenMatcher matcher;

        public SingleTokenValueContainer(string tokenMarker, object mapValue, ITokenMatcher tokenMatcher)
        {
            if (string.IsNullOrEmpty(tokenMarker)) throw new ArgumentNullException(nameof(tokenMarker));
            token = tokenMatcher.RemoveTokenMarkers(tokenMarker);
            value = mapValue;
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped)
        {
            if (matcher.TokenNameComparer.Equals(token, matchedToken.Token))
            {
                mapped = value;
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
