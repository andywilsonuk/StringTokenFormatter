using System;

namespace StringTokenFormatter
{
    public class TokenToFunctionStringValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            if (value is Func<string, string> func)
            {
                mapped = func(token.Token);
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
