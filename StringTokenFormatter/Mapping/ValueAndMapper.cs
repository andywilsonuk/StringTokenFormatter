using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class ValueAndMapper
    {
        public ValueAndMapper(IMatchedToken token, object value, ITokenToValueMapper mapper)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IMatchedToken Token { get; private set; }
        public object Value { get; private set; }
        public ITokenToValueMapper Mapper { get; private set; }

        public object Map() => Mapper.Map(Token, Value);
    }
}
