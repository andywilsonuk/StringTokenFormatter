using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToFunctionStringValueMapper : ITokenToValueMapper
    {
        private Func<string, string> mapper;

        public TokenToFunctionStringValueMapper(Func<string, string> mapperFunction)
        {
            mapper = mapperFunction ?? throw new ArgumentNullException(nameof(mapperFunction));
        }

        public object Map(IMatchedToken token)
        {
            return mapper(token.Token);
        }
    }
}
