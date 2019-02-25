using System;

namespace StringTokenFormatter
{
    public class TokenToFunctionObjectNoInputValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            if (value is Func<object> func)
            {
                mapped = func();
                return true;
            }
            mapped = null;
            return false;
        }
    }
}
