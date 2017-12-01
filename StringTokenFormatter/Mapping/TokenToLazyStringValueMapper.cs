using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToLazyStringValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            var lazy = value as Lazy<string>;
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
