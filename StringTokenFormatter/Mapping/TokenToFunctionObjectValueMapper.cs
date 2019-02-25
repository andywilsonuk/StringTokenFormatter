using System;

namespace StringTokenFormatter
{
    public class TokenToFunctionObjectValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            if (value is Func<string, object> func)
            {
                mapped = func(token.Token);
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
