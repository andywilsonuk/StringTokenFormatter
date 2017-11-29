using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToLazyObjectValueMapper : ITokenToValueMapper
    {
        private Lazy<object> mapper;

        public TokenToLazyObjectValueMapper(Lazy<object> lazyMapper)
        {
            mapper = lazyMapper ?? throw new ArgumentNullException(nameof(lazyMapper));
        }

        public object Map(IMatchedToken token)
        {
            return mapper.Value;
        }
    }
}
