using System;

namespace StringTokenFormatter
{
    public class TokenToFunctionStringNoInputValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            if (value is Func<string> func)
            {
                mapped = func();
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
