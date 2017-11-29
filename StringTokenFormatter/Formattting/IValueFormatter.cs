using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public interface IValueFormatter
    {
        string Format(TokenMatchingSegment token, object value);
    }
}
