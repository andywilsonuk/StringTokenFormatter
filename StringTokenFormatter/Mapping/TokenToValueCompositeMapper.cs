using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public class TokenToValueCompositeMapper : ITokenToValueMapper
    {
        private IEnumerable<ITokenToValueMapper> mappers;

        public TokenToValueCompositeMapper(IEnumerable<ITokenToValueMapper> tokenValueMappers)
        {
            mappers = tokenValueMappers ?? throw new ArgumentNullException(nameof(tokenValueMappers));
        }

        public bool TryMap(IMatchedToken matchedToken, object value, out object mapped)
        {
            foreach (var mapper in mappers)
            {
                if (mapper.TryMap(matchedToken, value, out mapped))
                {
                    return true;
                }
            }
            mapped = null;
            return false;
        }
    }
}
