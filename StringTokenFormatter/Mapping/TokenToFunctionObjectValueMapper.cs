using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToFunctionObjectValueMapper : ITokenToValueMapper
    {
        private Func<string, object> mapper;

        public TokenToFunctionObjectValueMapper(Func<string, object> mapperFunction)
        {
            this.mapper = mapperFunction ?? throw new ArgumentNullException(nameof(mapperFunction));
        }

        public object Map(IMatchedToken token)
        {
            return mapper(token.Token);
        }
    }
}
