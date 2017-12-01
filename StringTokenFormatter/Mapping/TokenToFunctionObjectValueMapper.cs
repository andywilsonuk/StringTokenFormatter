using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToFunctionObjectValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            var func = value as Func<string, object>;
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
