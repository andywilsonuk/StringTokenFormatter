using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public interface ITokenToValueMapper
    {
        object Map(IMatchedToken token);
    }
}
