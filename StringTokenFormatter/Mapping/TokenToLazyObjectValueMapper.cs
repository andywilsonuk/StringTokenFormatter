using System;

namespace StringTokenFormatter
{
    public class TokenToLazyObjectValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            var lazy = value as Lazy<object>;
            if (lazy == null)
            {
                mapped = null;
                return false;
            }
            mapped = lazy.Value;
            return true;
        }
    }
}
