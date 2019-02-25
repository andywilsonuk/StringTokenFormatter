using System;

namespace StringTokenFormatter
{
    public class TokenToLazyObjectValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            if (value is Lazy<object> lazy)
            {
                mapped = lazy.Value;
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
