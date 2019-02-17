using System.Collections.Generic;

namespace StringTokenFormatter
{
    public interface ITokenMatcher
    {
        SegmentedString SplitSegments(string input);
        string RemoveTokenMarkers(string token);
        IEqualityComparer<string> TokenNameComparer { get; }
    }
}
