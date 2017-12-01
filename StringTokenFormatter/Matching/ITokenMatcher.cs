using System.Collections.Generic;

namespace StringTokenFormatter
{
    public interface ITokenMatcher
    {
        IEnumerable<IMatchingSegment> SplitSegments(string input);
        string RemoveTokenMarkers(string token);
        IEqualityComparer<string> TokenNameComparer { get; }
    }
}
