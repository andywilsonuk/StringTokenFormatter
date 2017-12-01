using System;

namespace StringTokenFormatter
{
    public class TokenToFunctionStringValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            var func = value as Func<string, string>;
            if (func == null)
            {
                mapped = null;
                return false;
            }
            mapped = func(token.Token);
            return true;
        }
    }
}
