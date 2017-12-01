using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public interface ITokenToValueMapper
    {
        bool TryMap(IMatchedToken matchedToken, object value, out object mapped);
    }
}
