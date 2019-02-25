using System;

namespace StringTokenFormatter
{
    public class TokenToLazyStringValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            if (value is Lazy<string> lazy)
            {
                mapped = lazy.Value;
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
