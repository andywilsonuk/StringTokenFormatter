using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToLazyStringValueMapper : ITokenToValueMapper
    {
        private Lazy<string> mapper;

        public TokenToLazyStringValueMapper(Lazy<string> lazyMapper)
        {
            mapper = lazyMapper ?? throw new ArgumentNullException(nameof(lazyMapper));
        }

        public object Map(IMatchedToken token)
        {
            return mapper.Value;
        }
    }
}
