using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public interface ITokenMatcher
    {
        IEnumerable<IMatchingSegment> SplitSegments(string input); 
    }
}
