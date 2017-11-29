using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToLiteralValueMapper : ITokenToValueMapper
    {
        private object value;

        public TokenToLiteralValueMapper(object literalValue)
        {
            value = literalValue;
        }

        public object Map(IMatchedToken token)
        {
            return value;
        }
    }
}
